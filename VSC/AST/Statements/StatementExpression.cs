namespace VSC.AST {
public class StatementExpression : Statement
	{
		ExpressionStatement expr;
		
		public StatementExpression (ExpressionStatement expr)
		{
			this.expr = expr;
			loc = expr.Location;
		}

		public StatementExpression (ExpressionStatement expr, Location loc)
		{
			this.expr = expr;
			this.loc = loc;
		}

		public ExpressionStatement Expr {
			get {
 				return this.expr;
			}
		}
	
	}


}