using System.Collections.Generic;

namespace VSC.AST
{
    class CollectionElementInitializer : Invocation
    {
        public class ElementInitializerArgument : Argument
        {
            public ElementInitializerArgument(Expression e)
                : base(e, e.Location)
            {
            }
        }
        public CollectionElementInitializer(Expression argument)
            : base(null, new Arguments(1))
        {
            base.arguments.Add(new ElementInitializerArgument(argument));
            this.loc = argument.Location;
        }

        public CollectionElementInitializer(List<Expression> arguments, Location loc)
            : base(null, new Arguments(arguments.Count))
        {
            foreach (Expression e in arguments)
                base.arguments.Add(new ElementInitializerArgument(e));

            this.loc = loc;
        }

        public CollectionElementInitializer(Location loc)
            : base(null, null)
        {
            this.loc = loc;
        }

    }
}