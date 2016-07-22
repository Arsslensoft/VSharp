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

        public override Expression DoResolve(TypeSystem.Resolver.ResolveContext rc)
        {
            //// constructor argument list in collection initializer
            //Expression[] addArguments = new Expression[arguments.Count];
            //int i = 0;
            //foreach (var addArgument in arguments.FilterArgs)
            //    addArguments[i++] = addArgument.DoResolve(rc);

            //MemberLookup memberLookup = rc.CreateMemberLookup();
            //var addRR = memberLookup.Lookup(initializedObject, "Add", EmptyList<IType>.Instance, true);
            //var mgrr = addRR as MethodGroupExpression;
            //if (mgrr != null)
            //{
            //    OverloadResolution or = mgrr.PerformOverloadResolution(rc.Compilation, addArguments, null, false, false, false, resolver.CheckForOverflow, resolver.conversions);
            //    var invocationRR = or.CreateInvocation(initializedObject);
            //    initializerStatements.Add(invocationRR);
            //}
            return this;
        }
      

    }
}