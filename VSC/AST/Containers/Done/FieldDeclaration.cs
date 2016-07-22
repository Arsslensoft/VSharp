using System.Threading.Tasks;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class FieldDeclaration : FieldContainer, IUnresolvedField
    {
       
       
       	// <summary>
		//   Modifiers allowed in a class declaration
		// </summary>
		const Modifiers AllowedFieldModifiers =
			Modifiers.NEW |
			Modifiers.PUBLIC |
			Modifiers.PROTECTED |
			Modifiers.INTERNAL |
			Modifiers.PRIVATE |
			Modifiers.STATIC |
			Modifiers.READONLY;


  

          // For declarators
        public FieldDeclaration(FieldContainer baseconstant, MemberName name, Modifiers allowed)
            : base(baseconstant,name,allowed, SymbolKind.Field)
        {
            
        }
        public FieldDeclaration(FieldContainer baseconstant, MemberName name)
            : base(baseconstant, name, AllowedFieldModifiers, SymbolKind.Field)
        {

        }

        public FieldDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mods, MemberName name, VSharpAttributes attr)
            : this(parent, type, mods, AllowedFieldModifiers, name, attr)
        {

        }

        public FieldDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr)
            : base(parent,type,mods, allowed, name , attr,SymbolKind.Field)
        {
  }
   

        public override bool DoResolve(ResolveContext rc)
        {
       
            
            ResolvedField = Resolve(rc.CurrentTypeResolveContext, SymbolKind, name) as ResolvedFieldSpec;
            ResolvedField.ConstantLocation = Location;
            // do checks
            if (!base.DoResolve(rc))
                return false;


            if (ResolvedField.ConstantValue != null)
            {
                AST.Expression rr = ResolvedField.constantValue;
                if (rr.IsError)
                    rc.Report.Error(196, Location, "Cannot convert source type `{0}' to target type `{1}'", rr.Type.ToString(), ResolvedField.ReturnType.ToString());

            }
            else if (Initializer != null && Initializer.UnresolvedTypeReference != null)
                rc.Report.Error(196, Location, "Cannot convert source type `{0}' to target type `{1}'", Initializer.UnresolvedTypeReference.ToString(), ResolvedField.ReturnType.ToString());
            else if (Initializer != null )
                rc.Report.Error(196, Location, "Cannot convert value to target type `{0}'", ResolvedField.ReturnType.ToString());

            return true;
        }
        protected override void CheckTypeDependency(ResolveContext rc)
        {
            if ((ModFlags & Modifiers.BACKING_FIELD) != 0)
                return;

            base.CheckTypeDependency(rc);

        }

    


    }
}
