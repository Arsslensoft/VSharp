using System.Collections.Generic;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;

namespace VSC.AST
{
    public class CollectionOrObjectInitializers : ExpressionStatement
    {
        IList<Expression> initializers;
        bool is_collection_initialization;

        public CollectionOrObjectInitializers(Location loc)
            : this(new Expression[0], loc)
        {
        }

        public CollectionOrObjectInitializers(IList<Expression> initializers, Location loc)
        {
            this.initializers = initializers;
            this.loc = loc;
        }

        public IList<Expression> Initializers
        {
            get
            {
                return initializers;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return initializers.Count == 0;
            }
        }

        public bool IsCollectionInitializer
        {
            get
            {
                return is_collection_initialization;
            }
        }

        public override VSC.AST.Expression DoResolve(VSC.TypeSystem.Resolver.ResolveContext rc)
        {
            List<string> element_names = null;
            for (int i = 0; i < initializers.Count; ++i)
            {
                Expression initializer = initializers[i];
                ElementInitializer element_initializer = initializer as ElementInitializer;

                if (i == 0)
                {
                    if (element_initializer != null)
                    {
                        element_names = new List<string>(initializers.Count);
                        element_names.Add(element_initializer.Name);
                    }
                    //else if (initializer is CompletingExpression)//TODO:Add complete
                    //{
                    //    initializer.Resolve(ec);
                    //    throw new InternalErrorException("This line should never be reached");
                    //}
                    else
                    {
                        var t = rc.CurrentObjectInitializerType;
                        // LAMESPEC: The collection must implement IEnumerable only, no dynamic support
                        if (!t.Implements(KnownTypeReference.IEnumerable.Resolve(rc)))
                        {
                            rc.Report.Error(0, loc, "A field or property `{0}' cannot be initialized with a collection " +
                                "object initializer because type `{1}' does not implement `{2}' interface",
                                rc.CurrentObjectInitializer.GetSignatureForError(),
                                rc.CurrentObjectInitializerType.ToString(),
                                KnownTypeReference.IEnumerable.ToString());
                            return null;
                        }
                        is_collection_initialization = true;
                    }
                }
                else
                {
                    if (is_collection_initialization != (element_initializer == null))
                    {
                        rc.Report.Error(0, initializer.Location, "Inconsistent `{0}' member declaration",
                            is_collection_initialization ? "collection initializer" : "object initializer");
                        continue;
                    }

                    if (!is_collection_initialization)
                    {
                        if (element_names.Contains(element_initializer.Name))
                        {
                            rc.Report.Error(0, element_initializer.Location,
                                "An object initializer includes more than one member `{0}' initialization",
                                element_initializer.Name);
                        }
                        else
                        {
                            element_names.Add(element_initializer.Name);
                        }
                    }
                }

                Expression e = initializer.DoResolve(rc);
                if (e == EmptyExpressionStatement.Instance)
                    initializers.RemoveAt(i--);
                else
                    initializers[i] = e;
            }

            ResolvedType = rc.CurrentObjectInitializerType;
            if (is_collection_initialization)
            {
                if (ResolvedType is ElementTypeSpec)
                {
                    rc.Report.Error(0, loc, "Cannot initialize object of type `{0}' with a collection initializer",
                        ResolvedType.ToString());
                }
            }

            eclass = ExprClass.Variable;
            return this;
        }
    }
}