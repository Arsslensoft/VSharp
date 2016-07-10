using System.Collections.Generic;
using VSC.Context;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class PackageContainer : UsingScope, IAstNode, IResolve
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
            _imports = new List<Import>();
            _containers = new List<TypeContainer>();
            _ncontainers = new List<PackageContainer>();
            DefaultType = new TypeContainer(this,new MemberName( "default",loc),loc,file);
            DefaultType.IsPartial = true;
            DefaultType.IsSealed = true;
            DefaultType.IsStatic = true;
            DefaultType.IsSynthetic = false; DeclarationFound = false;
           
           DefaultTypesContainers.Add(DefaultType);
            Parent = parent;
        }

        public PackageContainer(CompilationSourceFile file)
        {
            _imports = new List<Import>();
            _containers = new List<TypeContainer>();
            _ncontainers = new List<PackageContainer>();
            DefaultType = new TypeContainer(this, new MemberName("default", Location.Null), Location.Null,file);
            DefaultType.IsPartial = true;
           DefaultType.IsSealed = true;
            DefaultType.IsStatic = true;
            DefaultType.IsSynthetic = false; DeclarationFound = false;
           
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
                i.Resolve(rc);

            foreach (var c in _containers)
                c.Resolve(rc);

            foreach (var n in _ncontainers)
                n.Resolve(rc);

        }

    
        public bool Resolve(ResolveContext rc)
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
    }
}