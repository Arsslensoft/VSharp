namespace VSC.AST {

	
	//
	// For statements which require special handling when inside try or catch block
	//
	public abstract class ExitStatement : Statement
	{
		/*protected bool unwind_protect;

		protected abstract bool DoResolve (BlockContext bc);
		protected abstract bool IsLocalExit { get; }

		public override bool Resolve (BlockContext bc)
		{
			var res = DoResolve (bc);

			if (!IsLocalExit) {
				//
				// We are inside finally scope but is it the scope we are exiting
				//
				if (bc.HasSet (ResolveContext.Options.FinallyScope)) {

					for (var b = bc.CurrentBlock; b != null; b = b.Parent) {
						if (b.IsFinallyBlock) {
							Error_FinallyClauseExit (bc);
							break;
						}

						if (b is ParametersBlock)
							break;
					}
				}
			}

			unwind_protect = bc.HasAny (ResolveContext.Options.TryScope | ResolveContext.Options.CatchScope);
			return res;
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			if (IsLocalExit)
				return true;

			if (fc.TryFinally != null) {
			    fc.TryFinally.RegisterForControlExitCheck (new DefiniteAssignmentBitSet (fc.DefiniteAssignment));
			} else {
			    fc.ParametersBlock.CheckControlExit (fc);
			}

			return true;
		}*/
	}


}