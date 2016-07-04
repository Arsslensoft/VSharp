namespace VSC.AST {

public abstract class SwitchGoto : Statement
	{
		/*protected bool unwind_protect;
		protected Switch switch_statement;

		protected SwitchGoto (Location loc)
		{
			this.loc = loc;
		}

		protected override void CloneTo (CloneContext clonectx, Statement target)
		{
			// Nothing to clone
		}

		public override bool Resolve (BlockContext bc)
		{
			CheckExitBoundaries (bc, bc.Switch.Block);

			unwind_protect = bc.HasAny (ResolveContext.Options.TryScope | ResolveContext.Options.CatchScope);
			switch_statement = bc.Switch;

			return true;
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			return true;
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			base.MarkReachable (rc);
			return Reachability.CreateUnreachable ();
		}

		protected void Error_GotoCaseRequiresSwitchBlock (BlockContext bc)
		{
			bc.Report.Error (153, loc, "A goto case is only valid inside a switch statement");
		}*/
	}
	
}