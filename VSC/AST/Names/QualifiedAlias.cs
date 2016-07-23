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

     
                AliasNamespace target = new AliasNamespace(alias, Location);
                AST.Expression targetRR = target.Resolve(resolver);
                if (targetRR.IsError)
                    return targetRR;
                IList<IType> typeArgs = typeArgumentsrefs.Resolve(resolver.CurrentTypeResolveContext);
                if (LookForAttribute)
                {
                    var wa = ResolveMemberAccess(resolver, targetRR, name+"Attribute", typeArgs, lookupMode);
                    if (wa == null || wa.IsError)
                    {
                        wa = ResolveMemberAccess(resolver, targetRR, name, typeArgs, lookupMode);
                        if (wa == null || wa.IsError)
                            resolver.Report.Error(6, loc, "This name `{0}' does not exist in the current context", name);
                    }
                    LookForAttribute = false;
                    return wa;
                }
                else
                {
                    var wa = ResolveMemberAccess(resolver, targetRR, name, typeArgs, lookupMode);
                    if (wa == null || wa.IsError)
                        resolver.Report.Error(6, loc, "This name `{0}' does not exist in the current context", name);

                    return wa;
                }
           
            

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