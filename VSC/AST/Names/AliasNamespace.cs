using System;
using VSC.Base;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    /// Looks up an alias (identifier in front of :: operator).
    /// </summary>
    /// <remarks>
    /// The member lookup performed by the :: operator is handled
    /// by <see cref="MemberTypeOrNamespaceReference"/>.
    /// </remarks>
    [Serializable]
    public sealed class AliasNamespace : TypeNameExpression, ISupportsInterning
    {

        public AliasNamespace(string identifier, Location l)
            : base(identifier,l)
        {
       
        }

        public override Expression DoResolve(ResolveContext rc)
        {
            Result = rc.ResolveAlias(name);
            if (Result.IsError)
            {
                if (Result is AmbiguousTypeResolveResult)
                    rc.Report.Error(7, loc, "The name `{0}' is ambigious", name);
                else if (Result is ErrorResolveResult)
                    rc.Report.Error(6, loc, "The name `{0}' does not exist in the current context", Name);
                else rc.Report.Error(6, loc, "The name `{0}' does not exist in the current context", name);
            }
            return this;
        }

        public override ResolveResult Resolve(ResolveContext resolver)
        {
            return resolver.ResolveAlias(name);
        }

        public override IType ResolveType(ResolveContext resolver)
        {
            // alias cannot refer to types
            return SpecialTypeSpec.UnknownType;
        }

        public override string ToString()
        {
            return name + "::";
        }

        int ISupportsInterning.GetHashCodeForInterning()
        {
            return name.GetHashCode();
        }

        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            AliasNamespace anr = other as AliasNamespace;
            return anr != null && this.name == anr.name;
        }
    }
}