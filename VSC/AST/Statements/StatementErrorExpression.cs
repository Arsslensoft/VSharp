namespace VSC.AST {
public class StatementErrorExpression : Statement
	{
	Expression expr;

		public StatementErrorExpression (Expression expr)
		{
			this.expr = expr;
			this.loc = expr.Location;
		}

		public Expression Expr {
			get {
				return expr;
			}
		}

	}



}