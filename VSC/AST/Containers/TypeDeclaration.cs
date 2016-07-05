using System.Text;
using System.Threading.Tasks;
using VSC.TypeSystem;

namespace VSC.AST
{
    public class TypeDeclaration : TypeContainer
    {


        public const string DefaultIndexerName = "Item";
        public TypeDeclaration(PackageContainer ns, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr, Location l, TypeKind kind, CompilationSourceFile file) 
            : base(ns,name,l,file)
        {
            if (attr != null)
                this.AddAttributes(attr);
            this.mod_flags = mods;
            this.mod_flags = MemberContainer.Check(allowed, mods, Modifiers.INTERNAL, l);
            ApplyModifiers(mods);
           
            this.Kind = kind;
        }
        public TypeDeclaration(TypeContainer ns, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr, Location l, TypeKind kind, CompilationSourceFile file)
            : base(ns, name, l,file)
        {
            if (attr != null)
                this.AddAttributes(attr);
            this.mod_flags = mods;
            this.mod_flags =  MemberContainer.Check(allowed, mods, ns.Name == "default" ? Modifiers.INTERNAL : Modifiers.PRIVATE, l);
            ApplyModifiers(mods);
            this.Kind = kind;
        }
        public ParametersCompiled PrimaryConstructorParameters { get; set; }
        public Arguments PrimaryConstructorBaseArguments { get; set; }
        public Location PrimaryConstructorBaseArgumentsStart { get; set; }
        public ToplevelBlock PrimaryConstructorBlock { get; set; }
    }
}
