using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class CompilationSourceFile : FreezableSpec,IUnresolvedFile, IAstNode, IResolve
    {

        readonly UsingScope rootUsingScope;
        IList<IUnresolvedTypeDefinition> topLevelTypeDefinitions = new List<IUnresolvedTypeDefinition>();
        IList<IUnresolvedAttribute> assemblyAttributes = new List<IUnresolvedAttribute>();
        IList<IUnresolvedAttribute> moduleAttributes = new List<IUnresolvedAttribute>();
        IList<UsingScope> usingScopes = new List<UsingScope>();
        IList<ErrorMessage> errors = new List<ErrorMessage>();
        Dictionary<IUnresolvedEntity, string> documentation;


        public PackageContainer RootPackage { get; set; }
        readonly SourceFile file;
        Dictionary<string, SourceFile> include_files;
        Dictionary<string, bool> conditionals;
        public CompilerContext Compiler { get; set; }
        public CompilationSourceFile(CompilerContext compiler, SourceFile sourceFile)
        {
            this.Compiler = compiler;
            this.file = sourceFile;
            RootPackage = new PackageContainer(this);
            rootUsingScope = RootPackage;
        }

        public IList<TypeContainer> Containers
        {
            get
            {
                return this.TopLevelTypeDefinitions.Cast<TypeContainer>().ToList();
            }
        }
        public IDictionary<string, bool> Conditionals
        {
            get
            {
                return conditionals ?? new Dictionary<string, bool>();
            }
        }

        public SourceFile SourceFile
        {
            get
            {
                return file;
            }
        }
        public void AddAttributes(VSharpAttributes attr)
        {
           if(attr  != null)
            foreach(var att in attr.Attrs)
                moduleAttributes.Add(att);


        }
        public void AddAttributes(VSharpAttribute att)
        {
           
                    moduleAttributes.Add(att);


        }
        public void AddIncludeFile(SourceFile file)
        {
            if (file == this.file)
                return;

            if (include_files == null)
                include_files = new Dictionary<string, SourceFile>();

            if (!include_files.ContainsKey(file.FullPathName))
                include_files.Add(file.FullPathName, file);
        }

        public void AddDefine(string value)
        {
            if (conditionals == null)
                conditionals = new Dictionary<string, bool>(2);

            conditionals[value] = true;
        }

        public void AddUndefine(string value)
        {
            if (conditionals == null)
                conditionals = new Dictionary<string, bool>(2);

            conditionals[value] = false;
        }





        public bool IsConditionalDefined(string value)
        {
            if (conditionals != null)
            {
                bool res;
                if (conditionals.TryGetValue(value, out res))
                    return res;

                // When conditional was undefined
                if (conditionals.ContainsKey(value))
                    return false;
            }

            return Compiler.Settings.Symbols.Contains(value);
        }

        public void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }


        public object DoResolve(ResolveContext rc)
        {
            throw new NotImplementedException();
        }

        public bool Resolve(ResolveContext rc)
        {
            foreach (var c in Containers)
                c.Resolve(rc);

            RootPackage.Resolve(rc);
            return true;
        }



        #region UNRESOLVED FILE
        protected override void FreezeInternal()
        {
            base.FreezeInternal();
            rootUsingScope.Freeze();
            topLevelTypeDefinitions = FreezableHelper.FreezeListAndElements(topLevelTypeDefinitions);
            assemblyAttributes = FreezableHelper.FreezeListAndElements(assemblyAttributes);
            moduleAttributes = FreezableHelper.FreezeListAndElements(moduleAttributes);
            usingScopes = FreezableHelper.FreezeListAndElements(usingScopes);
        }

        public string FileName
        {
            get { return file.FullPathName; }
          
        }

        DateTime? lastWriteTime;

        public DateTime? LastWriteTime
        {
            get { return lastWriteTime; }
            set
            {
                FreezableHelper.ThrowIfFrozen(this);
                lastWriteTime = value;
            }
        }
        public UsingScope RootUsingScope
        {
            get { return rootUsingScope; }
        }
        public IList<UsingScope> UsingScopes
        {
            get { return usingScopes; }
        }

        public IList<IUnresolvedTypeDefinition> TopLevelTypeDefinitions
        {
            get { return topLevelTypeDefinitions; }
        }

        public IList<IUnresolvedAttribute> AssemblyAttributes
        {
            get { return assemblyAttributes; }
        }

        public IList<IUnresolvedAttribute> ModuleAttributes
        {
            get { return moduleAttributes; }
        }

        public void AddDocumentation(IUnresolvedEntity entity, string xmlDocumentation)
        {
            FreezableHelper.ThrowIfFrozen(this);
            if (documentation == null)
                documentation = new Dictionary<IUnresolvedEntity, string>();
            documentation.Add(entity, xmlDocumentation);
        }

        public UsingScope GetUsingScope(Location location)
        {
            foreach (UsingScope scope in usingScopes)
            {
                if (scope.Region.IsInside(location.Line, location.Column))
                    return scope;
            }
            return rootUsingScope;
        }

        public IUnresolvedTypeDefinition GetTopLevelTypeDefinition(Location location)
        {
            return FindEntity(topLevelTypeDefinitions, location);
        }

        public IUnresolvedTypeDefinition GetInnermostTypeDefinition(Location location)
        {
            IUnresolvedTypeDefinition parent = null;
            IUnresolvedTypeDefinition type = GetTopLevelTypeDefinition(location);
            while (type != null)
            {
                parent = type;
                type = FindEntity(parent.NestedTypes, location);
            }
            return parent;
        }

        public IUnresolvedMember GetMember(Location location)
        {
            IUnresolvedTypeDefinition type = GetInnermostTypeDefinition(location);
            if (type == null)
                return null;
            return FindEntity(type.Members, location);
        }

        static T FindEntity<T>(IList<T> list, Location location) where T : class, IUnresolvedEntity
        {
            // This could be improved using a binary search
            foreach (T entity in list)
            {
                if (entity.Region.IsInside(location.Line, location.Column))
                    return entity;
            }
            return null;
        }

        public VSharpTypeResolveContext GetTypeResolveContext(ICompilation compilation, Location loc)
        {
            var rctx = new VSharpTypeResolveContext(compilation.MainAssembly);
            rctx = rctx.WithUsingScope(GetUsingScope(loc).Resolve(compilation));
            var curDef = GetInnermostTypeDefinition(loc);
            if (curDef != null)
            {
                var resolvedDef = curDef.Resolve(rctx).GetDefinition();
                if (resolvedDef == null)
                    return rctx;
                rctx = rctx.WithCurrentTypeDefinition(resolvedDef);

                var curMember = resolvedDef.Members.FirstOrDefault(m => m.Region.FileName == FileName && m.Region.Begin <= loc && loc < m.BodyRegion.End);
                if (curMember != null)
                    rctx = rctx.WithCurrentMember(curMember);
            }

            return rctx;
        }

        public ResolveContext GetResolver(ICompilation compilation, Location loc)
        {
            return new ResolveContext(GetTypeResolveContext(compilation, loc));
        }

        public string GetDocumentation(IUnresolvedEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (documentation == null)
                return null;
            string xmlDoc;
            if (documentation.TryGetValue(entity, out xmlDoc))
                return xmlDoc;
            else
                return null;
        }
		
        #endregion
    }
}
