namespace VSC.AST {
//
	// Base class for blocks using exception handling
	//
	public abstract class ExceptionStatement : ResumableStatement
	{
		

		protected ExceptionStatement parent;

		protected ExceptionStatement (Location loc)
		{
			this.loc = loc;
		}

	}


}