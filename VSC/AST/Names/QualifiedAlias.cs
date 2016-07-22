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
           return Resolve(rc);

        }

        #region ITypeReference
        public override Expression Resolve(ResolveContext resolver)
        {
            return this;
            //if (Result == null || Result.IsError)
            //{
            //    AliasNamespace target = new AliasNamespace(alias, Location);
            //    AST.Expression targetRR = target.Resolve(resolver);
            //    if (targetRR.IsError)
            //        return targetRR;
            //    IList<IType> typeArgs = typeArgumentsrefs.Resolve(resolver.CurrentTypeResolveContext);
            //    Result = LookForAttribute ? resolver.ResolveMemberAccess(targetRR, name + "Attribute", typeArgs, lookupMode) : resolver.ResolveMemberAccess(targetRR, name, typeArgs, lookupMode);
            //    if ((Result == null || Result.IsError) && LookForAttribute)
            //        Result = resolver.ResolveMemberAccess(targetRR, name, typeArgs, lookupMode);
            //}

            //if (Result.IsError)
            //    resolver.Report.Error(148, loc, "Type `{0}' does not contain a definition for `{1}' and no extension method `{1}' of type `{0}' could be found.", expr.GetSignatureForError(), GetSignatureForError());           

            //return Result;
        }
        public override string ToString()
        {
            return GetSignatureForError();
        }
        public override IType ResolveType(ResolveContext resolver)
        {
            TypeExpression trr = Resolve(resolver) as TypeExpression;
            return trr != null ? trr.Type : new UnknownTypeSpec(Alias, name, typeArgumentsrefs.Count);
        }
        #endregion
    }
}