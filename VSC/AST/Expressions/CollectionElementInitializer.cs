using System.Collections.Generic;
using VSC.Base;
using VSC.TypeSystem.Resolver;

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
        sealed class AddMemberAccess : MemberAccess
        {
            public AddMemberAccess(Expression expr, Location loc)
                : base(expr, "Add", loc)
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

        public override Expression DoResolve (ResolveContext ec)
		{
			base.expr = new AddMemberAccess (ec.CurrentObjectInitializer, loc);
			return base.DoResolve (ec);
		}
      

    }
}