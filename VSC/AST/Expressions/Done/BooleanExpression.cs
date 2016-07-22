using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class BooleanExpression : ShimExpression
    {
        public BooleanExpression(Expression expr)
            : base(expr)
        {
            this.loc = expr.Location;
        }
        public override Expression DoResolve(ResolveContext rc)
        {
            // A boolean-expression is required to be of a type
            // that can be implicitly converted to bool or of
            // a type that implements operator true
            eclass = ExprClass.Value;
            expr = expr.DoResolve(rc);
            if (expr == null)
                return null;

            Assign ass = expr as Assign;
            if (ass != null && ass.Source is Constant)
                rc.Report.Warning(247, 3, loc,
                    "Assignment in conditional expression is always constant. Did you mean to use `==' instead ?");
            

            if ((expr.Type as ITypeDefinition).KnownTypeCode == KnownTypeCode.Boolean)
                return expr;


            //TODO:Dynamic support
            //if (expr.Type.BuiltinType == BuiltinTypeSpec.Type.Dynamic)
            //{
            //    Arguments args = new Arguments(1);
            //    args.Add(new Argument(expr));
            //    return DynamicUnaryConversion.CreateIsTrue(ec, args, loc).Resolve(ec);
            //}

            ResolvedType = KnownTypeReference.Boolean.Resolve(rc);
            return rc.ResolveCondition(expr).DoResolve(rc);

        
        }
    }
}