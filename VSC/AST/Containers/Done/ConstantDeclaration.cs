using System;
using VSC.Base;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    [Serializable]
    public class ConstantDeclaration : FieldContainer
    {
        const Modifiers AllowedModifiers =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE;
        public ConstantDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mods, MemberName name, VSharpAttributes attr)
            : base(parent, type, mods, AllowedModifiers, name, attr, SymbolKind.Field)
        {
            mod_flags |= Modifiers.STATIC;
            IsStatic = true;
        }
        public ConstantDeclaration(TypeContainer parent, ITypeReference type, Modifiers mods, MemberName name, VSharpAttributes attr)
            : base(parent, new TypeExpression(type, name.Location), mods, AllowedModifiers, name, attr, SymbolKind.Field)
        {
            mod_flags |= Modifiers.STATIC;
            IsStatic = true;
          

        }
        // For declarators
        public ConstantDeclaration(ConstantDeclaration baseconstant, MemberName name)
            : base(baseconstant, name, AllowedModifiers, SymbolKind.Field)
        {
            mod_flags |= Modifiers.STATIC;
            IsStatic = true;
        }
        public bool IsConstantCompatible(IType type) {
	
				if (type.Kind == TypeKind.Class || type.Kind == TypeKind.Array || type.Kind == TypeKind.Delegate || type.Kind == TypeKind.Enum || type.Kind == TypeKind.Interface)
                    return true;
                else
				    return type.IsBuiltinType();
			
		}

        public override bool DoResolve(ResolveContext rc)
        {
       


            ResolvedField = Resolve(rc.CurrentTypeResolveContext, SymbolKind, name) as ResolvedFieldSpec;
            ResolvedField.ConstantLocation = Location;
            // do checks
            if (!base.DoResolve(rc))
                return false;


            if (!IsConstantCompatible(ResolvedField.ReturnType))
            {

                if (ResolvedField.ReturnType is TypeParameterSpec)
                    rc.Report.Error(197, Location,
                        "Type parameter `{0}' cannot be declared const", ResolvedField.ReturnType.ToString());

                else
                    rc.Report.Error(198, Location,
                        "The type `{0}' cannot be declared const", ResolvedField.ReturnType.ToString());

            }
            return false;
        }
   
     
    }
}