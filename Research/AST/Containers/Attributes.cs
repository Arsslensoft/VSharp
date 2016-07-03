using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{

    public class VSharpAttribute : IUnresolvedAttribute
    {
        public readonly string ExplicitTarget;
        public AttributeTargets Target;
        readonly TypeNameExpression attributeType;

        Arguments pos_args, named_args;
        readonly Location loc;
        public VSharpAttribute(string target, TypeNameExpression expr, Arguments[] args, Location loc, bool nameEscaped)
        {
            this.attributeType = expr;
            if (args != null)
            {
                pos_args = args[0];
                this.positionalArguments = pos_args.ToPositionalArgs();
                if (args[1] != null)
                {
                    this.namedCtorArguments = named_args.ToNamedCtorArgs();
                    this.namedArguments = named_args.ToNamedArgs();
                }
                named_args = args[1];
            }
            this.loc = loc;
            ExplicitTarget = target;

         
            this.positionalArguments = positionalArguments ?? EmptyList<IConstantValue>.Instance;
            this.namedCtorArguments = namedCtorArguments ?? EmptyList<KeyValuePair<string, IConstantValue>>.Instance;
       
        }

        public Location Location
        {
            get
            {
                return loc;
            }
        }

        public Arguments NamedArguments
        {
            get
            {
                return named_args;
            }
        }
        public Arguments PositionalArguments
        {
            get
            {
                return pos_args;
            }
        }

        public TypeNameExpression AttributeType
        {
            get
            {
                return attributeType;
            }
        }
        public DomRegion Region
        {
            get { return new DomRegion(loc); }
        }

        #region UnresolvedAttribute

        DomRegion region;
        IList<IConstantValue> positionalArguments;
        IList<KeyValuePair<string, IConstantValue>> namedCtorArguments;
        IList<KeyValuePair<string, IConstantValue>> namedArguments;
        public IAttribute CreateResolvedAttribute(ITypeResolveContext context)
        {
            return new VSharpResolvedAttribute((VSharpTypeResolveContext)context, this);
        }

        sealed class VSharpResolvedAttribute : IAttribute
        {
            readonly VSharpTypeResolveContext context;
            readonly VSharpAttribute unresolved;
            readonly IType attributeType;

            IList<KeyValuePair<IMember, ResolveResult>> namedArguments;

            public VSharpResolvedAttribute(VSharpTypeResolveContext context, VSharpAttribute unresolved)
            {
                this.context = context;
                this.unresolved = unresolved;
                // Pretty much any access to the attribute checks the type first, so
                // we don't need to use lazy-loading for that.
                this.attributeType = (unresolved.AttributeType as ITypeReference).Resolve(context);
            }

            DomRegion IAttribute.Region
            {
                get { return unresolved.Region; }
            }

            IType IAttribute.AttributeType
            {
                get { return attributeType; }
            }

            ResolveResult ctorInvocation;

            InvocationResolveResult GetCtorInvocation()
            {
                ResolveResult rr = LazyInit.VolatileRead(ref this.ctorInvocation);
                if (rr != null)
                {
                    return rr as InvocationResolveResult;
                }
                else
                {
                    VSharpResolver resolver = new VSharpResolver(context);
                    int totalArgumentCount = unresolved.positionalArguments.Count + unresolved.namedCtorArguments.Count;
                    ResolveResult[] arguments = new ResolveResult[totalArgumentCount];
                    string[] argumentNames = new string[totalArgumentCount];
                    int i = 0;
                    while (i < unresolved.positionalArguments.Count)
                    {
                        IConstantValue cv = unresolved.positionalArguments[i];
                        arguments[i] = cv.Resolve(context);
                        i++;
                    }
                    foreach (var pair in unresolved.namedCtorArguments)
                    {
                        argumentNames[i] = pair.Key;
                        arguments[i] = pair.Value.Resolve(context);
                        i++;
                    }
                    rr = resolver.ResolveObjectCreation(attributeType, arguments, argumentNames);
                    return LazyInit.GetOrSet(ref this.ctorInvocation, rr) as InvocationResolveResult;
                }
            }

            IMethod IAttribute.Constructor
            {
                get
                {
                    var invocation = GetCtorInvocation();
                    if (invocation != null)
                        return invocation.Member as IMethod;
                    else
                        return null;
                }
            }

            IList<ResolveResult> positionalArguments;

            IList<ResolveResult> IAttribute.PositionalArguments
            {
                get
                {
                    var result = LazyInit.VolatileRead(ref this.positionalArguments);
                    if (result != null)
                    {
                        return result;
                    }
                    else
                    {
                        var invocation = GetCtorInvocation();
                        if (invocation != null)
                            result = invocation.GetArgumentsForCall();
                        else
                            result = EmptyList<ResolveResult>.Instance;
                        return LazyInit.GetOrSet(ref this.positionalArguments, result);
                    }
                }
            }

            IList<KeyValuePair<IMember, ResolveResult>> IAttribute.NamedArguments
            {
                get
                {
                    var namedArgs = LazyInit.VolatileRead(ref this.namedArguments);
                    if (namedArgs != null)
                    {
                        return namedArgs;
                    }
                    else
                    {
                        namedArgs = new List<KeyValuePair<IMember, ResolveResult>>();
                        foreach (var pair in unresolved.namedArguments)
                        {
                            IMember member = attributeType.GetMembers(m => (m.SymbolKind == SymbolKind.Field || m.SymbolKind == SymbolKind.Property) && m.Name == pair.Key).FirstOrDefault();
                            if (member != null)
                            {
                                ResolveResult val = pair.Value.Resolve(context);
                                namedArgs.Add(new KeyValuePair<IMember, ResolveResult>(member, val));
                            }
                        }
                        return LazyInit.GetOrSet(ref this.namedArguments, namedArgs);
                    }
                }
            }
        }

        #endregion

    }
    public class VSharpAttributes
    {
        public readonly List<VSharpAttribute> Attrs;

        public VSharpAttributes(VSharpAttribute a)
        {
            Attrs = new List<VSharpAttribute>();
            Attrs.Add(a);
        }
       
        public VSharpAttributes(List<VSharpAttribute> attrs)
        {
            Attrs = attrs ?? new List<VSharpAttribute>();
        }

        public void AddAttribute(VSharpAttribute attr)
        {
            Attrs.Add(attr);
        }

        public void AddAttributes(List<VSharpAttribute> attrs)
        {
            Attrs.AddRange(attrs);
        }
    }
}
