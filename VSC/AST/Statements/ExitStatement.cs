namespace VSC.AST {

	
	//
	// For statements which require special handling when inside try or catch block
	//
	public abstract class ExitStatement : Statement
	{
	


		protected abstract bool IsLocalExit { get; }

	
	}


}