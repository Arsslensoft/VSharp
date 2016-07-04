namespace VSC.AST {
public class Sync : TryFinallyBlock
	{
		Expression expr;
		
		public Sync (Expression expr, Statement stmt, Location loc)
			: base (stmt, loc)
		{
			this.expr = expr;
		}

		public Expression Expr {
			get {
 				return this.expr;
			}
		}


	}
    

}