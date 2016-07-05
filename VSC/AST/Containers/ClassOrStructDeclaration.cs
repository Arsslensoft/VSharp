using VSC.TypeSystem;

namespace VSC.AST
{
    public class ClassOrStructDeclaration : TypeDeclaration
    {
        public ClassOrStructDeclaration(PackageContainer ns, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr, Location l, TypeKind kind, CompilationSourceFile file)
            : base(ns,mods,allowed, name,attr, l,kind,file)
        {

        }
        public ClassOrStructDeclaration(TypeContainer ns, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr, Location l, TypeKind kind, CompilationSourceFile file)
            : base(ns,mods,allowed, name, attr, l,kind,file)
        {

        }
    }
}