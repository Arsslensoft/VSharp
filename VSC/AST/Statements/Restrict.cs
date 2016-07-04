namespace VSC.AST {
 public class Restrict : TryFinallyBlock
    {
        Expression expr;
      
        public Restrict(Expression expr, Statement stmt, Location loc)
            : base(stmt, loc)
        {
            this.expr = expr;
        }

        public Expression Expr
        {
            get
            {
                return this.expr;
            }
        }
  }

    

}