namespace VSC.AST {

public class Goto : ExitStatement
	{
		/*string target;
		LabeledStatement label;
		TryFinally try_finally;

		public Goto (string label, Location l)
		{
			loc = l;
			target = label;
		}

		public string Target {
			get { return target; }
		}

		protected override bool IsLocalExit {
			get {
				return true;
			}
		}

		protected override bool DoResolve (BlockContext bc)
		{
			label = bc.CurrentBlock.LookupLabel (target);
			if (label == null) {
				Error_UnknownLabel (bc, target, loc);
				return false;
			}

			try_finally = bc.CurrentTryBlock as TryFinally;

			CheckExitBoundaries (bc, label.Block);

			return true;
		}

		public static void Error_UnknownLabel (BlockContext bc, string label, Location loc)
		{
			bc.Report.Error (159, loc, "The label `{0}:' could not be found within the scope of the goto statement",
				label);
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			// Goto to unreachable label
			if (label == null)
				return true;

			if (fc.AddReachedLabel (label))
				return true;

			label.Block.ScanGotoJump (label, fc);
			return true;
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			if (rc.IsUnreachable)
				return rc;

			base.MarkReachable (rc);

			if (try_finally != null) {
				if (try_finally.FinallyBlock.HasReachableClosingBrace) {
					label.AddGotoReference (rc);
				} else {
					label = null;
				}
			} else {
				label.AddGotoReference (rc);
			}

			return Reachability.CreateUnreachable ();
		}

		protected override void CloneTo (CloneContext clonectx, Statement target)
		{
			// Nothing to clone
		}

		protected override void DoEmit (EmitContext ec)
		{
			// This should only happen for goto from try block to unrechable label
			if (label == null)
				return;

			Label l = label.LabelTarget (ec);

			if (ec.TryFinallyUnwind != null && IsLeavingFinally (label.Block)) {
				var async_body = (AsyncInitializer) ec.CurrentAnonymousMethod;
				l = TryFinally.EmitRedirectedJump (ec, async_body, l, label.Block);
			}

			ec.Emit (unwind_protect ? OpCodes.Leave : OpCodes.Br, l);
		}

		bool IsLeavingFinally (Block labelBlock)
		{
			var b = try_finally.Statement as Block;
			while (b != null) {
				if (b == labelBlock)
					return true;

				b = b.Parent;
			}

			return false;
		}
		
		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}

}