namespace VSC.AST {
public class Throw : Statement {
		Expression expr;
		
		public Throw (Expression expr, Location l)
		{
			this.expr = expr;
			loc = l;
		}

		public Expression Expr {
			get {
 				return this.expr;
			}
		}

	}



}