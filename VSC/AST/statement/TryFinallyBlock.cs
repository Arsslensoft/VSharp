namespace VSC.AST {
public abstract class TryFinallyBlock : ExceptionStatement
	{
		/*protected Statement stmt;
		Label dispose_try_block;
		bool prepared_for_dispose, emitted_dispose;
		Method finally_host;
        IMethodData _methodData;
		protected TryFinallyBlock (Statement stmt, Location loc)
			: base (loc)
		{
			this.stmt = stmt;
		}
        protected TryFinallyBlock(Statement stmt,IMethodData md, Location loc)
            : base(loc)
        {
            _methodData = md;
            this.stmt = stmt;
        }
		#region Properties

		public Statement Statement {
			get {
				return stmt;
			}
		}

		#endregion

		protected abstract void EmitTryBody (EmitContext ec);
		public abstract void EmitFinallyBody (EmitContext ec);

		public override Label PrepareForDispose (EmitContext ec, Label end)
		{
			if (!prepared_for_dispose) {
				prepared_for_dispose = true;
				dispose_try_block = ec.DefineLabel ();
			}
			return dispose_try_block;
		}

		protected sealed override void DoEmit (EmitContext ec)
		{
			EmitTryBodyPrepare (ec);
			EmitTryBody (ec);

			bool beginFinally = EmitBeginFinallyBlock (ec);

			Label start_finally = ec.DefineLabel ();
			if (resume_points != null && beginFinally) {
				var state_machine = (StateMachineInitializer) ec.CurrentAnonymousMethod;

				ec.Emit (OpCodes.Ldloc, state_machine.SkipFinally);
				ec.Emit (OpCodes.Brfalse_S, start_finally);
				ec.Emit (OpCodes.Endfinally);
			}

			ec.MarkLabel (start_finally);

			if (finally_host != null) {
				finally_host.Define ();
				finally_host.PrepareEmit ();
				finally_host.Emit ();

				// Now it's safe to add, to close it properly and emit sequence points
				finally_host.Parent.AddMember (finally_host);

				var ce = new CallEmitter ();
				ce.InstanceExpression = new CompilerGeneratedThis (ec.CurrentType, loc);
				ce.EmitPredefined (ec, finally_host.Spec, new Arguments (0), true);
			} else {
				EmitFinallyBody (ec);
			}

			if (beginFinally)
				ec.EndExceptionBlock ();
		}

		public override void EmitForDispose (EmitContext ec, LocalBuilder pc, Label end, bool have_dispatcher)
		{
			if (emitted_dispose)
				return;

			emitted_dispose = true;

			Label end_of_try = ec.DefineLabel ();

			// Ensure that the only way we can get into this code is through a dispatcher
			if (have_dispatcher)
				ec.Emit (OpCodes.Br, end);

			ec.BeginExceptionBlock ();

			ec.MarkLabel (dispose_try_block);

			Label[] labels = null;
			for (int i = 0; i < resume_points.Count; ++i) {
				ResumableStatement s = resume_points[i];
				Label ret = s.PrepareForDispose (ec, end_of_try);
				if (ret.Equals (end_of_try) && labels == null)
					continue;
				if (labels == null) {
					labels = new Label[resume_points.Count];
					for (int j = 0; j < i; ++j)
						labels[j] = end_of_try;
				}
				labels[i] = ret;
			}

			if (labels != null) {
				int j;
				for (j = 1; j < labels.Length; ++j)
					if (!labels[0].Equals (labels[j]))
						break;
				bool emit_dispatcher = j < labels.Length;

				if (emit_dispatcher) {
					ec.Emit (OpCodes.Ldloc, pc);
					ec.EmitInt (first_resume_pc);
					ec.Emit (OpCodes.Sub);
					ec.Emit (OpCodes.Switch, labels);
				}

				foreach (ResumableStatement s in resume_points)
					s.EmitForDispose (ec, pc, end_of_try, emit_dispatcher);
			}

			ec.MarkLabel (end_of_try);

			ec.BeginFinallyBlock ();

			if (finally_host != null) {
				var ce = new CallEmitter ();
				ce.InstanceExpression = new CompilerGeneratedThis (ec.CurrentType, loc);
				ce.EmitPredefined (ec, finally_host.Spec, new Arguments (0), true);
			} else {
				EmitFinallyBody (ec);
			}

			ec.EndExceptionBlock ();
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			var res = stmt.FlowAnalysis (fc);
			parent = null;
			return res;
		}

		protected virtual bool EmitBeginFinallyBlock (EmitContext ec)
		{
			ec.BeginFinallyBlock ();
			return true;
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			base.MarkReachable (rc);
			return Statement.MarkReachable (rc);
		}

		public override bool Resolve (BlockContext bc)
		{
			bool ok;

			parent = bc.CurrentTryBlock;
			bc.CurrentTryBlock = this;
           
			using (bc.Set (ResolveContext.Options.TryScope)) {
			 ok = stmt.Resolve(bc);
			}

			bc.CurrentTryBlock = parent;

			//
			// Finally block inside iterator is called from MoveNext and
			// Dispose methods that means we need to lift the block into
			// newly created host method to emit the body only once. The
			// original block then simply calls the newly generated method.
			//
			if (bc.CurrentIterator != null && !bc.IsInProbingMode) {
				var b = stmt as Block;
				if (b != null && b.Explicit.HasYield) {
					finally_host = bc.CurrentIterator.CreateFinallyHost (this);
				}
			}

			return base.Resolve (bc) && ok;
		}*/
	}

	

}