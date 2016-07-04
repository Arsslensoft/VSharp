namespace VSC.AST {
	public class If : Statement {
		Expression expr;
		public Statement TrueStatement;
		public Statement FalseStatement;

		public If (Expression bool_expr, Statement true_statement, Location l)
			: this (bool_expr, true_statement, null, l)
		{
		}

		public If (Expression bool_expr,
			   Statement true_statement,
			   Statement false_statement,
			   Location l)
		{
			this.expr = bool_expr;
			TrueStatement = true_statement;
			FalseStatement = false_statement;
			loc = l;
		}

		public Expression Expr {
			get {
				return this.expr;
			}
		}
		
		
	}


}