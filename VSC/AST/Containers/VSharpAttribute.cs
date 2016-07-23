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

    public class VSharpAttribute : IUnresolvedAttribute, IResolve
    {
        public readonly string ExplicitTarget;
        public AttributeTargets Target;
        readonly TypeNameExpression attributeType;

        public IAttribute ResolvedAttribute;

        Arguments pos_args, named_args;
        readonly Location loc;
        public VSharpAttribute(string target, TypeNameExpression expr, Arguments[] args, Location loc, bool nameEscaped)
        {
            this.attributeType = expr;
            if (args != null)
            {
                pos_args = args[0];
                 pos_args.FilterArgs(out namedCtorArguments, out positionalArguments);
                named_args = args[1];
                if (args[1] != null)
                    this.namedArguments = named_args.ToNamedArgs();
                
             
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
            return new VSharpResolvedAttribute(context as ResolveContext, this);
        }

       public sealed class VSharpResolvedAttribute : IAttribute
        {
            readonly ResolveContext context;
            readonly VSharpAttribute unresolved;
            readonly IType attributeType;

            IList<KeyValuePair<IMember, AST.Expression>> namedArguments;

            public VSharpResolvedAttribute(ResolveContext context, VSharpAttribute unresolved)
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

          public  AST.Expression ctorInvocation;

          public  Invocation GetCtorInvocation()
            {
                AST.Expression rr = LazyInit.VolatileRead(ref this.ctorInvocation);
                if (rr != null)
                {
                    return rr as Invocation;
                }
                else
                {
              
                    int totalArgumentCount = unresolved.positionalArguments.Count + unresolved.namedCtorArguments.Count;
                    AST.Expression[] arguments = new AST.Expression[totalArgumentCount];
                    string[] argumentNames = new string[totalArgumentCount];
                    int i = 0;
                    while (i < unresolved.positionalArguments.Count)
                    {
                        IConstantValue cv = unresolved.positionalArguments[i];
                        arguments[i] = cv.ResolveConstant(context);
                        i++;
                    }
                    foreach (var pair in unresolved.namedCtorArguments)
                    {
                        argumentNames[i] = pair.Key;
                        arguments[i] = pair.Value.ResolveConstant(context);
                        i++;
                    }

                    rr = NewExpression.ResolveObjectCreation(context,unresolved.loc,attributeType, arguments, argumentNames);
                    return LazyInit.GetOrSet(ref this.ctorInvocation, rr) as Invocation;
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

            IList<AST.Expression> positionalArguments;

            IList<AST.Expression> IAttribute.PositionalArguments
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
                            result = EmptyList<AST.Expression>.Instance;
                        return LazyInit.GetOrSet(ref this.positionalArguments, result);
                    }
                }
            }

            IList<KeyValuePair<IMember, AST.Expression>> IAttribute.NamedArguments
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
                        namedArgs = new List<KeyValuePair<IMember, AST.Expression>>();
                        foreach (var pair in unresolved.namedArguments)
                        {
                            IMember member = attributeType.GetMembers(m => (m.SymbolKind == SymbolKind.Field || m.SymbolKind == SymbolKind.Property) && m.Name == pair.Key).FirstOrDefault();
                            if (member != null)
                            {
                                AST.Expression val = pair.Value.ResolveConstant(context);
                                namedArgs.Add(new KeyValuePair<IMember, AST.Expression>(member, val));
                            }
                        }
                        return LazyInit.GetOrSet(ref this.namedArguments, namedArgs);
                    }
                }
            }
        }

        #endregion




        public bool DoResolve(ResolveContext rc)
        {
            AttributeType.LookForAttribute = true;
            ResolvedAttribute = CreateResolvedAttribute(rc);
            if (ResolvedAttribute.AttributeType is UnknownTypeSpec)
                rc.Report.Error(151, Location, "The attribute type `{0}' does not exist in the current context",
                    AttributeType.GetSignatureForError());
            else
            {
                if (ResolvedAttribute.AttributeType is ResolvedTypeDefinitionSpec)
                {
                    var t = ResolvedAttribute.AttributeType as ResolvedTypeDefinitionSpec;
                    if(t.IsAbstract)
                        rc.Report.Error(168, Location, "Cannot apply attribute class `{0}' because it is abstract", ResolvedAttribute.AttributeType.ToString());
           
                    if(t.IsStatic)
                        rc.Report.Error(169, Location, "Cannot apply attribute class `{0}' because it is static", ResolvedAttribute.AttributeType.ToString());


                }
                bool is_attrib = false;
                foreach(IType b in ResolvedAttribute.AttributeType.DirectBaseTypes)
                    if (b.FullName == "Std.Attribute")
                    {
                        is_attrib = true;
                        break;  
                    }

                if(!is_attrib)
                    rc.Report.Error(170, Location, "<`{0}': is not an attribute class", ResolvedAttribute.AttributeType.ToString());

                // ctor checking
                var ctor = (ResolvedAttribute as VSharpResolvedAttribute).GetCtorInvocation();
                if(ctor != null && ctor.IsError)
                    rc.Report.Error(171, Location, "A constructor has been found for `{0}' but an error has occured", ResolvedAttribute.AttributeType.ToString());
                else if(ctor == null)
                    rc.Report.Error(172, Location, "No suitable constructor has been found for `{0}'", ResolvedAttribute.AttributeType.ToString());
            }
            return true;
            
        }
    }
}
