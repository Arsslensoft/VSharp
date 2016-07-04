namespace VSC.AST {
	public class For : LoopStatement
	{

		
		public For (Location l)
			: base (null)
		{
			loc = l;
		}

		public Statement Initializer {
			get; set;
		}

		public Expression Condition {
			get; set;
		}

		public Statement Iterator {
			get; set;
		}

	}


}