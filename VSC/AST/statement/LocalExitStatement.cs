namespace VSC.AST {
public abstract class LocalExitStatement : ExitStatement
	{
		/*protected LoopStatement enclosing_loop;

		protected LocalExitStatement (Location loc)
		{
			this.loc = loc;
		}

		protected override bool IsLocalExit {
			get {
				return true;
			}
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			// nothing needed.
		}

		protected override bool DoResolve (BlockContext bc)
		{
			if (enclosing_loop == null) {
				bc.Report.Error (139, loc, "No enclosing loop out of which to break or continue");
				return false;
			}

			var block = enclosing_loop.Statement as Block;

			// Don't need to do extra checks for simple statements loops
			if (block != null) {
				CheckExitBoundaries (bc, block);
			}

			return true;
		}*/
	}


}