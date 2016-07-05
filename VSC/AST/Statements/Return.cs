namespace VSC.AST {
/// <summary>
	///   Implements the return statement
	/// </summary>
	public class Return : ExitStatement
	{
		Expression expr;

		public Return (Expression expr, Location l)
		{
			this.expr = expr;
			loc = l;
		}

		#region Properties

		public Expression Expr {
			get {
				return expr;
			}
			protected set {
				expr = value;
			}
		}

		protected override bool IsLocalExit {
			get {
				return false;
			}
		}

		#endregion

	}
    //
	// This is a return statement that is prepended lambda expression bodies that happen
	// to be expressions.  Depending on the return type of the delegate this will behave
	// as either { expr (); return (); } or { return expr (); }
	//
}