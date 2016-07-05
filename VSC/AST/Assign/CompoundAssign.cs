using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class CompoundAssign : Assign
    {
        // Used for underlying binary operator
        readonly BinaryOperatorType op;
        Expression right;
        Expression left;

        public CompoundAssign(BinaryOperatorType op, Expression target, Expression source)
            : base(target, source, target.Location)
        {
            right = source;
            this.op = op;
        }

        public CompoundAssign(BinaryOperatorType op, Expression target, Expression source, Expression left)
            : this(op, target, source)
        {
            this.left = left;
        }

        public BinaryOperatorType Operator
        {
            get
            {
                return op;
            }
        }

    }
}