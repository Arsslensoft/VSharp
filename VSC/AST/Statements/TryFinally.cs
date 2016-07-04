namespace VSC.AST {
public class TryFinally : TryFinallyBlock
	{
		ExplicitBlock fini;


		public TryFinally (Statement stmt, ExplicitBlock fini, Location loc)
			 : base (stmt, loc)
		{
			this.fini = fini;
		}

		public ExplicitBlock FinallyBlock {
			get {
 				return fini;
			}
		}

	
	}

	

}