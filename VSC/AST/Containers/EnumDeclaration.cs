using System.Collections.Generic;
using System.Linq;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{

    public sealed class EnumDeclaration : TypeDeclaration
    {
        /// <summary>
        ///   Modifiers allowed in a enum declaration
        /// </summary>
        const Modifiers AllowedModifiers =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE;
        public EnumDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mod_flags, MemberName name, VSharpAttributes attrs, CompilationSourceFile file)
            : base(parent, mod_flags, AllowedModifiers, name, attrs, name.Location, TypeKind.Enum, file)
        {
            if(type != null)
            SetBaseTypes(type);
            mod_flags |= Modifiers.SEALED;
        }
        public static readonly string UnderlyingValueField = "value__";
        public override void AddMember(MemberContainer member)
        {
            if (member.Name == UnderlyingValueField)
            {
                Report.Error(0, member.Location, "An item in an enumeration cannot have an identifier `{0}'",
                    UnderlyingValueField);
                return;
            }
            base.AddMember(member);
        }

        public override void ResolveWithCurrentContext(ResolveContext rc)
        {
            base.ResolveWithCurrentContext(rc);
            // resolve enum members
            foreach (var m in TypeMembers)
                (m as IResolve).DoResolve(rc);


        }
    }
}