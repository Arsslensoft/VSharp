namespace VSC.AST {
public class TryFinally : TryFinallyBlock
	{
		/*ExplicitBlock fini;
		List<DefiniteAssignmentBitSet> try_exit_dat;
		List<Label> redirected_jumps;
		Label? start_fin_label;

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

		public void RegisterForControlExitCheck (DefiniteAssignmentBitSet vector)
		{
			if (try_exit_dat == null)
				try_exit_dat = new List<DefiniteAssignmentBitSet> ();

			try_exit_dat.Add (vector);
		}

		public override bool Resolve (BlockContext bc)
		{
			bool ok = base.Resolve (bc);

			fini.SetFinallyBlock ();
			using (bc.Set (ResolveContext.Options.FinallyScope)) {
				ok &= fini.Resolve (bc);
			}

			return ok;
		}

		protected override void EmitBeginException (EmitContext ec)
		{
			if (fini.HasAwait && stmt is TryCatch)
				ec.BeginExceptionBlock ();

			base.EmitBeginException (ec);
		}

		protected override void EmitTryBody (EmitContext ec)
		{
			if (fini.HasAwait) {
				if (ec.TryFinallyUnwind == null)
					ec.TryFinallyUnwind = new List<TryFinally> ();

				ec.TryFinallyUnwind.Add (this);
				stmt.Emit (ec);

				if (stmt is TryCatch)
					ec.EndExceptionBlock ();

				ec.TryFinallyUnwind.Remove (this);

				if (start_fin_label != null)
					ec.MarkLabel (start_fin_label.Value);

				return;
			}

			stmt.Emit (ec);
		}

		protected override bool EmitBeginFinallyBlock (EmitContext ec)
		{
			if (fini.HasAwait)
				return false;

			return base.EmitBeginFinallyBlock (ec);
		}

		public override void EmitFinallyBody (EmitContext ec)
		{
			if (!fini.HasAwait) {
				fini.Emit (ec);
				return;
			}

			//
			// Emits catch block like
			//
			// catch (object temp) {
			//	this.exception_field = temp;
			// }
			//
			var type = ec.BuiltinTypes.Object;
			ec.BeginCatchBlock (type);

			var temp = ec.GetTemporaryLocal (type);
			ec.Emit (OpCodes.Stloc, temp);

			var exception_field = ec.GetTemporaryField (type);
			ec.EmitThis ();
			ec.Emit (OpCodes.Ldloc, temp);
			exception_field.EmitAssignFromStack (ec);

			ec.EndExceptionBlock ();

			ec.FreeTemporaryLocal (temp, type);

			fini.Emit (ec);

			//
			// Emits exception rethrow
			//
			// if (this.exception_field != null)
			//	throw this.exception_field;
			//
			exception_field.Emit (ec);
			var skip_throw = ec.DefineLabel ();
			ec.Emit (OpCodes.Brfalse_S, skip_throw);
			exception_field.Emit (ec);
			ec.Emit (OpCodes.Throw);
			ec.MarkLabel (skip_throw);

			exception_field.IsAvailableForReuse = true;

			EmitUnwindFinallyTable (ec);
		}

		bool IsParentBlock (Block block)
		{
			for (Block b = fini; b != null; b = b.Parent) {
				if (b == block)
					return true;
			}

			return false;
		}

		public static Label EmitRedirectedJump (EmitContext ec, AsyncInitializer initializer, Label label, Block labelBlock)
		{
			int idx;
			if (labelBlock != null) {
				for (idx = ec.TryFinallyUnwind.Count; idx != 0; --idx) {
					var fin = ec.TryFinallyUnwind [idx - 1];
					if (!fin.IsParentBlock (labelBlock))
						break;
				}
			} else {
				idx = 0;
			}

			bool set_return_state = true;

			for (; idx < ec.TryFinallyUnwind.Count; ++idx) {
				var fin = ec.TryFinallyUnwind [idx];
				if (labelBlock != null && !fin.IsParentBlock (labelBlock))
					break;

				fin.EmitRedirectedExit (ec, label, initializer, set_return_state);
				set_return_state = false;

				if (fin.start_fin_label == null) {
					fin.start_fin_label = ec.DefineLabel ();
				}

				label = fin.start_fin_label.Value;
			}

			return label;
		}

		public static Label EmitRedirectedReturn (EmitContext ec, AsyncInitializer initializer)
		{
			return EmitRedirectedJump (ec, initializer, initializer.BodyEnd, null);
		}

		void EmitRedirectedExit (EmitContext ec, Label label, AsyncInitializer initializer, bool setReturnState)
		{
			if (redirected_jumps == null) {
				redirected_jumps = new List<Label> ();

				// Add fallthrough label
				redirected_jumps.Add (ec.DefineLabel ());

				if (setReturnState)
					initializer.HoistedReturnState = ec.GetTemporaryField (ec.Module.Compiler.BuiltinTypes.Int, true);
			}

			int index = redirected_jumps.IndexOf (label);
			if (index < 0) {
				redirected_jumps.Add (label);
				index = redirected_jumps.Count - 1;
			}

			//
			// Indicates we have captured exit jump
			//
			if (setReturnState) {
				var value = new IntConstant (initializer.HoistedReturnState.Type, index, Location.Null);
				initializer.HoistedReturnState.EmitAssign (ec, value, false, false);
			}
		}

		//
		// Emits state table of jumps outside of try block and reload of return
		// value when try block returns value
		//
		void EmitUnwindFinallyTable (EmitContext ec)
		{
			if (redirected_jumps == null)
				return;

			var initializer = (AsyncInitializer)ec.CurrentAnonymousMethod;
			initializer.HoistedReturnState.EmitLoad (ec);
			ec.Emit (OpCodes.Switch, redirected_jumps.ToArray ());

			// Mark fallthrough label
			ec.MarkLabel (redirected_jumps [0]);
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			var da = fc.BranchDefiniteAssignment ();

			var tf = fc.TryFinally;
			fc.TryFinally = this;

			var res_stmt = Statement.FlowAnalysis (fc);

			fc.TryFinally = tf;

			var try_da = fc.DefiniteAssignment;
			fc.DefiniteAssignment = da;

			var res_fin = fini.FlowAnalysis (fc);

			if (try_exit_dat != null) {
				//
				// try block has global exit but we need to run definite assignment check
				// for parameter block out parameter after finally block because it's always
				// executed before exit
				//
				foreach (var try_da_part in try_exit_dat)
					fc.ParametersBlock.CheckControlExit (fc, fc.DefiniteAssignment | try_da_part);

				try_exit_dat = null;
			}

			fc.DefiniteAssignment |= try_da;
			return res_stmt | res_fin;
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			//
			// Mark finally block first for any exit statement in try block
			// to know whether the code which follows finally is reachable
			//
			return fini.MarkReachable (rc) | base.MarkReachable (rc);
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			TryFinally target = (TryFinally) t;

			target.stmt = stmt.Clone (clonectx);
			if (fini != null)
				target.fini = (ExplicitBlock) clonectx.LookupBlock (fini);
		}
		
		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}

	

}