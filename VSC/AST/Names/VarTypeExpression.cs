using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class VarTypeExpression : SimpleName
    {
        public VarTypeExpression(Location loc)
            : base("var", loc)
        {
        }
        public bool InferType(ResolveContext ec, Expression right_side)
        {
            if (ResolvedType != null)
                throw new InternalErrorException("An implicitly typed local variable could not be redefined");

            ResolvedType = right_side.Type;
            if (ResolvedType.Kind == TypeKind.Void)
            {
                ec.Report.Error(0, loc,
                    "An implicitly typed local variable declaration cannot be initialized with `{0}'",
                    ResolvedType.ToString());
                return false;
            }

            eclass = ExprClass.Variable;
            return true;
        }

    }
}