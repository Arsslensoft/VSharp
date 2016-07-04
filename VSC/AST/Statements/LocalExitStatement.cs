namespace VSC.AST {
public abstract class LocalExitStatement : ExitStatement
	{


		protected LocalExitStatement (Location loc)
		{
			this.loc = loc;
		}

		protected override bool IsLocalExit {
			get {
				return true;
			}
		}

	}


}