using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public sealed class InterfaceDeclaration : TypeDeclaration
    {	
        /// <summary>
        ///   Modifiers allowed in a interface declaration
        /// </summary>
        const Modifiers AllowedModifiers =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE;
        public InterfaceDeclaration(TypeContainer parent, MemberName name, Modifiers mod, VSharpAttributes attrs, Location l, CompilationSourceFile file)
            : base(parent, mod, AllowedModifiers, name, attrs, l, TypeKind.Interface,file)
        {
            mod_flags |= Modifiers.ABSTRACT;
            IsAbstract = true;
        }

        public override void ResolveWithCurrentContext(ResolveContext rc)
        {
            // DoResolve nested types
            foreach (var t in TypeContainers)
                t.DoResolve(rc);
            // DoResolve members
            foreach (var m in TypeMembers)

                // resolve method
                (m as IResolve).DoResolve(rc);

        }

    
    }
}