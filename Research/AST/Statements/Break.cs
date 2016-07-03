namespace VSC.AST {
public class Break : LocalExitStatement
	{		
		/*public Break (Location l)
			: base (l)
		{
		}
		
		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}

		protected override void DoEmit (EmitContext ec)
		{
			var l = ec.LoopEnd;

			if (ec.TryFinallyUnwind != null) {
				var async_body = (AsyncInitializer) ec.CurrentAnonymousMethod;
				l = TryFinally.EmitRedirectedJump (ec, async_body, l, enclosing_loop.Statement as Block);
			}

			ec.Emit (unwind_protect ? OpCodes.Leave : OpCodes.Br, l);
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			enclosing_loop.AddEndDefiniteAssignment (fc);
			return true;
		}

		protected override bool DoResolve (BlockContext bc)
		{
			enclosing_loop = bc.EnclosingLoopOrSwitch;
			return base.DoResolve (bc);
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			base.MarkReachable (rc);

			if (!rc.IsUnreachable)
				enclosing_loop.SetEndReachable ();

			return Reachability.CreateUnreachable ();
		}*/
	}


}