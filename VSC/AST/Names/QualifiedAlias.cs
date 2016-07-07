using System.Collections.Generic;
using VSC.Base;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   Implements the qualified-alias-member (::) expression.
    /// </summary>
    public class QualifiedAlias : MemberAccess
    {

        readonly string alias;

        public QualifiedAlias(string alias, string identifier, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
            : base(null, identifier, l, lookupMode)
        {
            this.alias = alias;
        }

        public QualifiedAlias(string alias, string identifier, TypeArguments targs, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
            : base (null, identifier, targs, l, lookupMode)
        {
            this.alias = alias;
      
        }

        public QualifiedAlias(string alias, string identifier, int arity, Location l, NameLookupMode lookupMode = NameLookupMode.Type)
            : base (null, identifier, arity, l,lookupMode)
        {
            this.alias = alias;
        }

        public string Alias {
            get {
                return alias;
            }
        }
  
        public override string GetSignatureForError()
        {
            string name = Name;
            if (targs != null)
                name = Name + "<" + targs.GetSignatureForError() + ">";


            return alias + "::" + name;
        }

        public override void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }

        public override Expression DoResolve(ResolveContext rc)
        {
            if (Result != null && !Result.IsError)
                return this;

            Result = Resolve(rc);
         
            return this;
        }

        #region ITypeReference
        public override ResolveResult Resolve(ResolveContext resolver)
        {
            if (Result == null || Result.IsError)
            {
                AliasNamespace target = new AliasNamespace(alias, Location);
                ResolveResult targetRR = target.Resolve(resolver);
                if (targetRR.IsError)
                    return targetRR;
                IList<IType> typeArgs = typeArgumentsrefs.Resolve(resolver.CurrentTypeResolveContext);
                Result = LookForAttribute ? resolver.ResolveMemberAccess(targetRR, name + "Attribute", typeArgs, lookupMode) : resolver.ResolveMemberAccess(targetRR, name, typeArgs, lookupMode);
                if ((Result == null || Result.IsError) && LookForAttribute)
                    Result = resolver.ResolveMemberAccess(targetRR, name, typeArgs, lookupMode);
            }
            return Result;
        }
        public override string ToString()
        {
            return GetSignatureForError();
        }
        public override IType ResolveType(ResolveContext resolver)
        {
            TypeResolveResult trr = Resolve(resolver) as TypeResolveResult;
            return trr != null ? trr.Type : new UnknownTypeSpec(Alias, name, typeArgumentsrefs.Count);
        }
        #endregion
    }
}