namespace VSC.AST
{
    /// <summary>
    ///   Binary operators
    /// </summary>
    public class BinaryExpression : Expression
    {
        readonly VSC.TypeSystem.Resolver.BinaryOperatorType oper;
        Expression left, right;

        public BinaryExpression(VSC.TypeSystem.Resolver.BinaryOperatorType oper, Expression left, Expression right)
            : this(oper, left, right, left.Location)
        {
        }

        public BinaryExpression(VSC.TypeSystem.Resolver.BinaryOperatorType oper, Expression left, Expression right, Location loc)
        {
            this.oper = oper;
            this.left = left;
            this.right = right;
            this.loc = loc;
        }

        #region Properties

        public VSC.TypeSystem.Resolver.BinaryOperatorType Oper
        {
            get
            {
                return oper;
            }
        }

        public Expression Left
        {
            get
            {
                return this.left;
            }
        }

        public Expression Right
        {
            get
            {
                return this.right;
            }
        }



        #endregion
    }
}