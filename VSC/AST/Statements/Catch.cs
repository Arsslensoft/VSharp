namespace VSC.AST {

public class Except : Statement
	{
		class CatchVariableStore : Statement
		{
			readonly Except ctch;

			public CatchVariableStore (Except ctch)
			{
				this.ctch = ctch;
			}
	
		}

		class FilterStatement : Statement
		{
			readonly Except ctch;

			public FilterStatement (Except ctch)
			{
				this.ctch = ctch;
			}
		}

		ExplicitBlock block;
		LocalVariable li;
		FullNamedExpression type_expr;
		

		public Except (ExplicitBlock block, Location loc)
		{
			this.block = block;
			this.loc = loc;
		}

		#region Properties

		public ExplicitBlock Block {
			get {
				return block;
			}
		}

		

		public Expression Filter {
			get; set;
		}

		public bool IsGeneral {
			get {
				return type_expr == null;
			}
		}

		public FullNamedExpression TypeExpression {
			get {
				return type_expr;
			}
			set {
				type_expr = value;
			}
		}

		public LocalVariable Variable {
			get {
				return li;
			}
			set {
				li = value;
			}
		}

		#endregion

		
	}

	
}