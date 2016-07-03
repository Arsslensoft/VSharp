using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.TypeSystem;

namespace VSC.AST
{
    public class TypeDeclaration : TypeContainer
    {

        public const string DefaultIndexerName = "Item";
        public TypeDeclaration(NamespaceContainer ns, MemberName name, VSharpAttributes attr, Location l) 
            : base(ns,name,l)
        {
            if (attr != null)
                this.AddAttributes(attr);
        }
        public TypeDeclaration(TypeContainer ns, MemberName name, VSharpAttributes attr, Location l)
            : base(ns, name, l)
        {
            if (attr != null)
                this.AddAttributes(attr);
        }
        public ParametersCompiled PrimaryConstructorParameters { get; set; }
        public Arguments PrimaryConstructorBaseArguments { get; set; }
        public Location PrimaryConstructorBaseArgumentsStart { get; set; }
        public ToplevelBlock PrimaryConstructorBlock { get; set; }
    }
    public class ClassOrStructDeclaration : TypeDeclaration
    {
        public ClassOrStructDeclaration(NamespaceContainer ns, MemberName name, VSharpAttributes attr,Location l)
            : base(ns, name,attr, l)
        {

        }
        public ClassOrStructDeclaration(TypeContainer ns, MemberName name, VSharpAttributes attr, Location l)
            : base(ns, name, attr, l)
        {

        }
    }
    public class StructDeclaration : ClassOrStructDeclaration
    {
        public StructDeclaration(TypeContainer parent, MemberName name, Modifiers mod, VSharpAttributes attrs, Location l)
		: base (parent, name, attrs, l)
		{

         }
    }
    public class ClassDeclaration : ClassOrStructDeclaration
    {
        public ClassDeclaration(TypeContainer parent, MemberName name, Modifiers mod, VSharpAttributes attrs, Location l)
            : base(parent, name, attrs, l)
        {

        }
    }
    public class InterfaceDeclaration : TypeDeclaration
    {
        public InterfaceDeclaration(TypeContainer parent, MemberName name, Modifiers mod, VSharpAttributes attrs, Location l)
            : base(parent, name, attrs, l)
        {

        }
    }
    public class EnumDeclaration : TypeDeclaration
    {
        public EnumDeclaration(TypeContainer parent, MemberName name, Modifiers mod, VSharpAttributes attrs, Location l)
            : base(parent, name, attrs, l)
        {

        }
    }
}
