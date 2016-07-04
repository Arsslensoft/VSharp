namespace VSC.AST {
public class Using : TryFinallyBlock
	{
		public class VariableDeclaration : BlockVariable
		{
			Statement dispose_call;

			public VariableDeclaration (FullNamedExpression type, LocalVariable li)
				: base (type, li)
			{
			}

			public VariableDeclaration (LocalVariable li, Location loc)
				: base (li)
			{
				reachable = true;
				this.loc = loc;
			}

			public VariableDeclaration (Expression expr)
				: base (null)
			{
				loc = expr.Location;
				Initializer = expr;
			}

			#region Properties

			public bool IsNested { get; private set; }

			#endregion

			
		}

		VariableDeclaration decl;

		public Using (VariableDeclaration decl, Statement stmt, Location loc)
			: base (stmt, loc)
		{
			this.decl = decl;
		}

		public Using (Expression expr, Statement stmt, Location loc)
			: base (stmt, loc)
		{
			this.decl = new VariableDeclaration (expr);
		}

		#region Properties

		public Expression Expr {
			get {
				return decl.Variable == null ? decl.Initializer : null;
			}
		}

		public BlockVariable Variables {
			get {
				return decl;
			}
		}

		#endregion

	}

	

}