namespace VSC.AST {
/// <summary>
	///   `goto case' statement
	/// </summary>
	public class GotoCase : SwitchGoto
	{
		Expression expr;
		
		public GotoCase (Expression e, Location l)
			: base (l)
		{
			expr = e;
		}

		public Expression Expr {
			get {
 				return expr;
			}
		}

		public SwitchLabel Label { get; set; }

	
	}



}