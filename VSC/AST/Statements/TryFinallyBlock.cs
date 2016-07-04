namespace VSC.AST {
public abstract class TryFinallyBlock : ExceptionStatement
	{
		protected Statement stmt;

		protected TryFinallyBlock (Statement stmt, Location loc)
			: base (loc)
		{
			this.stmt = stmt;
		}

		#region Properties

		public Statement Statement {
			get {
				return stmt;
			}
		}

		#endregion

	}

	

}