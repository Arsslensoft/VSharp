using System;
using System.Collections.Generic;
using System.Linq;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;

namespace VSC.AST
{
    [Serializable]
    public class MethodDeclaration : MethodOrOperator
    {
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

                        var tpar = new MethodTypeParameterWithInheritedConstraints(idx++, tp.Name);
                        tpar.ApplyInterningProvider(CompilerContext.InternProvider);
                        this.typeParameters.Add(tp);
                    }
                }
                else
                {
                    foreach (var tp in mn.TypeParameters.names)
                        this.typeParameters.Add(new UnresolvedTypeParameterSpec(SymbolKind.Method, idx++, tp.Name));
                }
            }
        }
        public MethodDeclaration(TypeContainer parent, FullNamedExpression returnType, Modifiers mod,
            MemberName name, ParametersCompiled parameters, VSharpAttributes attrs)
            :base(parent,returnType, mod,parent is InterfaceDeclaration ? AllowedModifiersInterface :
                parent is StructDeclaration ? AllowedModifiersStruct :
                    AllowedModifiersClass, name, parameters, attrs)
        {
            SetTypeParameters(name);
        }

    }
}