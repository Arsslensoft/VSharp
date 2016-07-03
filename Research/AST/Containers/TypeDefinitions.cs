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
        public TypeDeclaration(NamespaceContainer ns,Modifiers mods, Modifiers allowed,MemberName name, VSharpAttributes attr, Location l, TypeKind kind) 
            : base(ns,name,l)
        {
            if (attr != null)
                this.AddAttributes(attr);
            this.mod_flags = mods;
            this.mod_flags = MemberContainer.Check(allowed, mods, Modifiers.INTERNAL, l);
            ApplyModifiers(mods);
           
            this.Kind = kind;
        }
        public TypeDeclaration(TypeContainer ns, Modifiers allowed, Modifiers mods, MemberName name, VSharpAttributes attr, Location l, TypeKind kind)
            : base(ns, name, l)
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
    public class ClassOrStructDeclaration : TypeDeclaration
    {
        public ClassOrStructDeclaration(NamespaceContainer ns, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr, Location l, TypeKind kind)
            : base(ns,mods,allowed, name,attr, l,kind)
        {

        }
        public ClassOrStructDeclaration(TypeContainer ns, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr, Location l, TypeKind kind)
            : base(ns,mods,allowed, name, attr, l,kind)
        {

        }
    }
    public class StructDeclaration : ClassOrStructDeclaration
    {
        // <summary>
		//   Modifiers allowed in a struct declaration
		// </summary>
		const Modifiers AllowedModifiers =
			Modifiers.NEW       |
			Modifiers.PUBLIC    |
			Modifiers.PROTECTED |
			Modifiers.INTERNAL  |
			Modifiers.PRIVATE;


        public StructDeclaration(TypeContainer parent, MemberName name, Modifiers mod, VSharpAttributes attrs, Location l)
            : base(parent, mod | Modifiers.SEALED, AllowedModifiers, name, attrs, l, TypeKind.Struct)
		{

         }
    }
    public sealed class ClassDeclaration : ClassOrStructDeclaration
    {  
        /// <summary>
        ///   Modifiers allowed in a class declaration
        /// </summary>
        const Modifiers AllowedModifiers =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE |
            Modifiers.ABSTRACT |
            Modifiers.SEALED |
            Modifiers.STATIC;
        public ClassDeclaration(TypeContainer parent, MemberName name, Modifiers mod, VSharpAttributes attrs, Location l)
            : base(parent, mod, AllowedModifiers, name, attrs, l, TypeKind.Class)
        {

        }
    }
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
        public InterfaceDeclaration(TypeContainer parent, MemberName name, Modifiers mod, VSharpAttributes attrs, Location l)
            : base(parent, mod, AllowedModifiers, name, attrs, l, TypeKind.Interface)
        {

        }
    }
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
        public EnumDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mod_flags, MemberName name, VSharpAttributes attrs)
            : base(parent, mod_flags, AllowedModifiers, name, attrs, name.Location, TypeKind.Enum)
        {

        }
    }
    public sealed class DelegateDeclaration : TypeDeclaration
    {
        const Modifiers MethodModifiers = Modifiers.PUBLIC | Modifiers.VIRTUAL;

        const Modifiers AllowedModifiers =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE;


        FullNamedExpression ReturnType;
        readonly ParametersCompiled parameters;
        public DelegateDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mod_flags, MemberName name, ParametersCompiled param_list,
				 VSharpAttributes attrs)
            : base(parent, mod_flags | Modifiers.SEALED, AllowedModifiers, name, attrs, name.Location, TypeKind.Delegate)

		{
            BaseTypes.Add(KnownTypeReference.MulticastDelegate);
			this.ReturnType = type;
			parameters      = param_list;
            List<IUnresolvedParameter> uparameters = new List<IUnresolvedParameter>();
            foreach (Parameter p in param_list.parameters)
                uparameters.Add(p);


            AddDefaultMethodsToDelegate(type as ITypeReference, uparameters);


            if (attrs != null)
            {
                IUnresolvedMethod invokeMethod = (IUnresolvedMethod)Members.Single(m => m.Name == "Invoke");
                foreach (IUnresolvedAttribute attr in attrs.Attrs.Where(x => x.ExplicitTarget == "ret").ToList())
                    invokeMethod.ReturnTypeAttributes.Add(attr);
            }
            
		}
        static readonly IUnresolvedParameter delegateObjectParameter = MakeParameter(KnownTypeReference.Object, "object");
        static readonly IUnresolvedParameter delegateIntPtrMethodParameter = MakeParameter(KnownTypeReference.IntPtr, "method");
        static IUnresolvedParameter MakeParameter(ITypeReference type, string name)
        {
            Parameter p = new Parameter(type, name, Location.Null);
            p.Freeze();
            return p;
        }
        public void AddDefaultMethodsToDelegate(ITypeReference returnType, IEnumerable<IUnresolvedParameter> parameters)
        {
        
            if (returnType == null)
                throw new ArgumentNullException("returnType");
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            DomRegion region = Region;
            region = new DomRegion(region.FileName, region.BeginLine, region.BeginColumn); // remove end position

            MethodOrOperator invoke = new MethodOrOperator(this,"Invoke");
            invoke.Accessibility = Accessibility.Public;
            invoke.IsSynthetic = true;
            foreach (var p in parameters)
                invoke.Parameters.Add(p);
            invoke.ReturnType = returnType;
            invoke.Region = region;
            this.Members.Add(invoke);

            MethodOrOperator ctor = new MethodOrOperator(this, ".ctor");
            ctor.SymbolKind = SymbolKind.Constructor;
            ctor.Accessibility = Accessibility.Public;
            ctor.IsSynthetic = true;
            ctor.Parameters.Add(delegateObjectParameter);
            ctor.Parameters.Add(delegateIntPtrMethodParameter);
            ctor.ReturnType = this;
            ctor.Region = region;
            this.Members.Add(ctor);
        }

    }


}
