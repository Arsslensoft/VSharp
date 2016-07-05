using System;
using System.Collections.Generic;
using System.Linq;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    /// Represents a simple V# name. (a single non-qualified identifier with an optional list of type arguments)
    /// </summary>
    [Serializable]
    public class SimpleName : TypeNameExpression, ISupportsInterning
    {    
        public SimpleName(string name, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(name, l)
        {
            this.typeArgumentsrefs = EmptyList<ITypeReference>.Instance;
            this.lookupMode = lookupMode;
        }

        public SimpleName(string name, TypeArguments args, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(name, args, l)
        {
            this.typeArgumentsrefs = args != null ? args.ToTypeReferences(CompilerContext.InternProvider) : EmptyList<ITypeReference>.Instance.ToList();
            this.lookupMode = lookupMode;
        }

        public SimpleName(string name, int arity, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(name, arity, l)
        {
            this.typeArgumentsrefs = EmptyList<ITypeReference>.Instance;
            this.lookupMode = lookupMode;
        }

    
        public override void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }
        
       
        public override Expression DoResolve(ResolveContext rc)
        {
        
            if (name == null)
                rc.Report.Error(6, loc, "This name `{0}' does not exist in the current context", name);
          
            if (typeArgumentsrefs == null)
                throw new ArgumentNullException("typeArgumentsrefs");

            var typeArgs = typeArgumentsrefs.Resolve(rc.CurrentTypeResolveContext);
            Result = rc.LookupSimpleNameOrTypeName(name, typeArgs, lookupMode);

            if (Result.IsError)
            {
              if(Result is AmbiguousTypeResolveResult)
                  rc.Report.Error(7, loc, "The name `{0}' is ambigious", name);
                else if(Result is ErrorResolveResult)
                    rc.Report.Error(6, loc, "The name `{0}' does not exist in the current context", Name);
              else rc.Report.Error(6, loc, "The name `{0}' does not exist in the current context", name);
            }
            else if(Result is LocalResolveResult && typeArgs.Count > 0)
                rc.Report.Error(8, loc, "The name `{0}' does not need type arguments because it's a local name", name);
           
        
            

            return this;
        }

        #region ITypeReference
        readonly IList<ITypeReference> typeArgumentsrefs;
    

   
        public IList<ITypeReference> TypeArgumentsReferences
        {
            get { return typeArgumentsrefs; }
        }

        public NameLookupMode LookupMode
        {
            get { return lookupMode; }
        }

        /// <summary>
        /// Adds a suffix to the identifier.
        /// Does not modify the existing type reference, but returns a new one.
        /// </summary>
        public SimpleName AddSuffix(string suffix)
        {
            return new SimpleName(name + suffix, this.targs,Location, lookupMode);
        }

        public override ResolveResult Resolve(ResolveContext resolver)
        {
            return Result;
        }

        public override IType ResolveType(ResolveContext resolver)
        {
            TypeResolveResult trr = Resolve(resolver) as TypeResolveResult;
            return trr != null ? trr.Type : new UnknownTypeSpec(null, name,Arity);
        }

        public override string ToString()
        {
            if (typeArgumentsrefs.Count == 0)
                return name;
            else
                return name + "<" + string.Join(",", typeArgumentsrefs) + ">";
        }

        int ISupportsInterning.GetHashCodeForInterning()
        {
            int hashCode = 0;
            unchecked
            {
                hashCode += 1000000021 * name.GetHashCode();
                hashCode += 1000000033 * typeArgumentsrefs.GetHashCode();
                hashCode += 1000000087 * (int)lookupMode;
            }
            return hashCode;
        }

        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            SimpleName o = other as SimpleName;
            return o != null && this.name == o.Name
                   && this.typeArgumentsrefs == o.typeArgumentsrefs && this.lookupMode == o.lookupMode;
        }
        #endregion
    }
}