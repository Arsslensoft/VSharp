using System;
using System.Globalization;
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
        #region Resolved
         INamespace ns;
        public INamespace Namespace
        {
            get { return ns; }
        }

        public string NamespaceName
        {
            get { return ns.FullName; }
        }
        #endregion


        public AliasNamespace(INamespace nameNs, Location l)
            :base(nameNs.Name, l)
        {
            ns = nameNs;
            _resolved = true;
        }
        public AliasNamespace(string alias,INamespace nameNs, Location l)
            : base(alias, l)
        {
            ns = nameNs;
            _resolved = true;
        }
        public AliasNamespace(string alias, AliasNamespace nameNs)
            : base(alias, nameNs.loc)
        {
            ns = nameNs.Namespace;
            _resolved = true;
        }


        public AliasNamespace(string identifier, Location l)
            : base(identifier,l)
        {
       
        }

        public override Expression DoResolve(ResolveContext rc)
        {
            return Resolve(rc);
        }

        public override Expression Resolve(ResolveContext resolver)
        {
            // resolved cache
            if (_resolved)
                return this;

            if (name == "global")
            {
                _resolved = true;
                ns = resolver.compilation.RootNamespace;
                return this;
            }


            for (ResolvedUsingScope n = resolver.CurrentUsingScope; n != null; n = n.Parent)
            {
                if (n.ExternAliases.Contains(name))
                    return ResolveExternAlias(resolver, name);
                foreach (var pair in n.UsingAliases)
                {
                    if (pair.Key == name)
                    {
                        if (pair.Value is AliasNamespace)
                            return pair.Value;
                        else
                        {
                            resolver.Report.Error(6, loc, "The name `{0}' does not exist in the current context", name);
                            return ErrorResult;
                        }
                    }
                 
                }
            }

            resolver.Report.Error(6, loc, "The name `{0}' does not exist in the current context", name);
            return ErrorResult;
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


        Expression ResolveExternAlias(ResolveContext rc, string alias)
        {
            INamespace ns = rc.compilation.GetNamespaceForExternAlias(alias);
            if (ns != null)
            {
                this.ns = ns;
                _resolved = true;
                return this;
            }
            else
                return ErrorResult;
        }
    }
}