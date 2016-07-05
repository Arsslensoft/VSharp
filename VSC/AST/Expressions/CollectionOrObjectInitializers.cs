using System.Collections.Generic;

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
    }
}