using System;
using System.Collections.Generic;
using System.Linq;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;

namespace VSC.AST
{
    [Serializable]
    public class MethodDeclaration : MethodCore
    {
        public override bool IsOverloadAllowed(MemberContainer overload)
        {
            if (overload is IndexerDeclaration)
                return false;

            return base.IsOverloadAllowed(overload);
        }

        public void SetTypeParameters(MemberName mn)
        {
            if (mn.TypeParameters != null)
            {
                int idx = 0;
                this.typeParameters = new List<IUnresolvedTypeParameter>();
                if (mn.ExplicitInterface != null || (mod_flags & Modifiers.OVERRIDE) != 0)
                {
                    foreach (var tp in mn.TypeParameters.names)
                    {
                        if (!typeParameters.Any(x => x.Name == tp.Name))
                        {
                            var tpar = new MethodTypeParameterWithInheritedConstraints(idx++, tp.Name, tp.Location);
                            tpar.ApplyInterningProvider(CompilerContext.InternProvider);
                            this.typeParameters.Add(tp);
                        }
                        else Report.Error(0, tp.Location,
       "Duplicate type parameter `{0}'", tp.ToString());
                    }
                }
                else
                {
                    this.typeParameters = new List<IUnresolvedTypeParameter>();
                    foreach (var tp in mn.TypeParameters.names)
                        if (!typeParameters.Any(x => x.Name == tp.Name))
                        this.typeParameters.Add(new UnresolvedTypeParameterSpec(SymbolKind.Method, idx++, tp.Location,tp.Name));
                        else Report.Error(0, tp.Location,
        "Duplicate type parameter `{0}'", tp.ToString());
                }
            }
        }

        
        public MethodDeclaration(TypeContainer parent, FullNamedExpression returnType, Modifiers mod,
            MemberName name, ParametersCompiled parameters, VSharpAttributes attrs)
            :base(parent,returnType, mod,parent is InterfaceDeclaration ? AllowedModifiersInterface :
                parent is StructDeclaration ? AllowedModifiersStruct :
                    AllowedModifiersClass, name, parameters, attrs, SymbolKind.Method)
        {
            SetTypeParameters(name);
        }

    }
}