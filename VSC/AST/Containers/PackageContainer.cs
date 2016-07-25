using System.Collections.Generic;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class PackageContainer : UsingScope, IAstNode, IResolve, IModuleSupport
    {
        public IAstNode ParentNode { get; set; }
        public static PackageContainer CreateContainers(PackageContainer parent, MemberName mn,Location loc, CompilationSourceFile file)
        {
            if (mn.Left != null)
            {
                PackageContainer left = CreateContainers(parent, mn.Left, loc, file);
                PackageContainer current = new PackageContainer(left, mn.Name, loc, file);
                left.AddChildNamespace(current);
                return current;
            }
            else
            {
             var c = new PackageContainer(parent, mn.Name, loc, file);
                parent.AddChildNamespace(c);
                return c;
            }
        }
        public bool DeclarationFound { get; set; }
        public TypeContainer DefaultType { get; set; }

        public static List<TypeContainer> DefaultTypesContainers = new List<TypeContainer>();
        public VSharpAttributes UnattachedAttributes;
        public PackageContainer Parent=null;
        public PackageContainer(PackageContainer parent, string name,Location loc, CompilationSourceFile file)
            : base(parent, name)
        {
            Parent = parent;
            _imports = new List<Import>();
            _containers = new List<TypeContainer>();
            _ncontainers = new List<PackageContainer>();
            DefaultType = new ClassDeclaration(this, new MemberName("default", Location.Null), Modifiers.PUBLIC | Modifiers.STATIC | Modifiers.SEALED | Modifiers.COMPILER_GENERATED, null, Location.Null, file);
            DeclarationFound = false;
           
           DefaultTypesContainers.Add(DefaultType);
          
        }

        public PackageContainer(CompilationSourceFile file, ModuleContext module)
        {
            _imports = new List<Import>();
            _containers = new List<TypeContainer>();
            this.module = module;
            _ncontainers = new List<PackageContainer>();
            DefaultType = new ClassDeclaration(this, new MemberName("default", Location.Null), Modifiers.PUBLIC | Modifiers.STATIC  | Modifiers.SEALED | Modifiers.COMPILER_GENERATED,null, Location.Null,file);
            DeclarationFound = false;
           
            DefaultTypesContainers.Add(DefaultType);
        }
        // only types
        private List<TypeContainer> _containers;
        public IList<TypeContainer> TypeContainers
        {
            get
            {
                return _containers;
            }
        }

        private List<Import> _imports;
        public IList<Import> Imports
        {
            get
            {
                return _imports;
            }
        }
        private List<PackageContainer> _ncontainers;
        public IList<PackageContainer> NamespaceContainers
        {
            get
            {
                return _ncontainers;
            }
        }
        public void AddChildNamespace(PackageContainer nc)
        {
            _ncontainers.Add(nc);
        }

        public void AddImport(Import imp)
        {
            _imports.Add(imp);
            var u = imp.NamespaceExpression.ToTypeReference(CompilerContext.InternProvider);
            if (imp.Alias == null)
                Usings.Add(u as TypeNameExpression);
            else UsingAliases.Add(new KeyValuePair<string, TypeNameExpression>(imp.Alias.Value, u as TypeNameExpression));
        }
     
        public void ResolveWithCurrentContext(ResolveContext rc)
        {
            foreach (var i in _imports)
                i.DoResolve(rc);

            foreach (var c in _containers)
                c.DoResolve(rc);

            foreach (var n in _ncontainers)
                n.DoResolve(rc);

        }

    
        public bool DoResolve(ResolveContext rc)
        {
            ResolveContext previousResolver = rc;
            try
            {

                rc = rc.WithCurrentUsingScope(this.ResolveScope(rc.Compilation));

                ResolveWithCurrentContext(rc);

            }
            finally
            {
                rc = previousResolver;
            }

            return true;
        }
 
        public virtual void AcceptVisitor(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        private ModuleContext module;
        public virtual ModuleContext Module
        {
            get
            {
                if (Parent != null)
                    return Parent.Module;
                else return module;
            }
            set { module = value; }
        }
    }
}