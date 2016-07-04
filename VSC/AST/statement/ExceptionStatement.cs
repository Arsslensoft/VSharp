namespace VSC.AST {
//
	// Base class for blocks using exception handling
	//
	public abstract class ExceptionStatement : ResumableStatement
	{
		/*protected List<ResumableStatement> resume_points;
		protected int first_resume_pc;
		protected ExceptionStatement parent;

		protected ExceptionStatement (Location loc)
		{
			this.loc = loc;
		}

		protected virtual void EmitBeginException (EmitContext ec)
		{
			ec.BeginExceptionBlock ();
		}

		protected virtual void EmitTryBodyPrepare (EmitContext ec)
		{
			StateMachineInitializer state_machine = null;
			if (resume_points != null) {
				state_machine = (StateMachineInitializer) ec.CurrentAnonymousMethod;

				ec.EmitInt ((int) IteratorStorey.State.Running);
				ec.Emit (OpCodes.Stloc, state_machine.CurrentPC);
			}

			EmitBeginException (ec);

			if (resume_points != null) {
				ec.MarkLabel (resume_point);

				// For normal control flow, we want to fall-through the Switch
				// So, we use CurrentPC rather than the $PC field, and initialize it to an outside value above
				ec.Emit (OpCodes.Ldloc, state_machine.CurrentPC);
				ec.EmitInt (first_resume_pc);
				ec.Emit (OpCodes.Sub);

				Label[] labels = new Label[resume_points.Count];
				for (int i = 0; i < resume_points.Count; ++i)
					labels[i] = resume_points[i].PrepareForEmit (ec);
				ec.Emit (OpCodes.Switch, labels);
			}
		}

		public virtual int AddResumePoint (ResumableStatement stmt, int pc, StateMachineInitializer stateMachine)
		{
			if (parent != null) {
				// TODO: MOVE to virtual TryCatch
				var tc = this as TryCatch;
				var s = tc != null && tc.IsTryCatchFinally ? stmt : this;

				pc = parent.AddResumePoint (s, pc, stateMachine);
			} else {
				pc = stateMachine.AddResumePoint (this);
			}

			if (resume_points == null) {
				resume_points = new List<ResumableStatement> ();
				first_resume_pc = pc;
			}

			if (pc != first_resume_pc + resume_points.Count)
				throw new InternalErrorException ("missed an intervening AddResumePoint?");

			resume_points.Add (stmt);
			return pc;
		}*/
	}


}