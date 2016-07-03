namespace VSC.AST {
public class TryCatch : ExceptionStatement
	{
/*		public Block Block;
		protected List<Catch> clauses;
		protected readonly bool inside_try_finally;
		protected List<Catch> catch_sm;

		public TryCatch (Block block, List<Catch> catch_clauses, Location l, bool inside_try_finally)
			: base (l)
		{
			this.Block = block;
			this.clauses = catch_clauses;
			this.inside_try_finally = inside_try_finally;
		}

		public List<Catch> Clauses {
			get {
				return clauses;
			}
		}

		public bool IsTryCatchFinally {
			get {
				return inside_try_finally;
			}
		}

		public override bool Resolve (BlockContext bc)
		{
			bool ok;

			using (bc.Set (ResolveContext.Options.TryScope)) {
				parent = bc.CurrentTryBlock;

				if (IsTryCatchFinally) {
					ok = Block.Resolve (bc);
				} else {
					using (bc.Set (ResolveContext.Options.TryWithCatchScope)) {
						bc.CurrentTryBlock = this;
						ok = Block.Resolve (bc);
						bc.CurrentTryBlock = parent;
					}
				}
			}

			for (int i = 0; i < clauses.Count; ++i) {
				var c = clauses[i];

				ok &= c.Resolve (bc);

				if (c.Block.HasAwait) {
					if (catch_sm == null)
						catch_sm = new List<Catch> ();

					catch_sm.Add (c);
				}

				if (c.Filter != null)
					continue;

				TypeSpec resolved_type = c.CatchType;
				if (resolved_type == null)
					continue;

				for (int ii = 0; ii < clauses.Count; ++ii) {
					if (ii == i)
						continue;

					if (clauses[ii].Filter != null)
						continue;

					if (clauses[ii].IsGeneral) {
						if (resolved_type.BuiltinType != BuiltinTypeSpec.Type.Exception)
							continue;

						if (!bc.Module.DeclaringAssembly.WrapNonExceptionThrows)
							continue;

						if (!bc.Module.PredefinedAttributes.RuntimeCompatibility.IsDefined)
							continue;

						bc.Report.Warning (1058, 1, c.loc,
							"A previous catch clause already catches all exceptions. All non-exceptions thrown will be wrapped in a `System.Runtime.CompilerServices.RuntimeWrappedException'");

						continue;
					}

					if (ii >= i)
						continue;

					var ct = clauses[ii].CatchType;
					if (ct == null)
						continue;

					if (resolved_type == ct || TypeSpec.IsBaseClass (resolved_type, ct, true)) {
						bc.Report.Error (160, c.loc,
							"A previous catch clause already catches all exceptions of this or a super type `{0}'",
							ct.GetSignatureForError ());
						ok = false;
					}
				}
			}

			return base.Resolve (bc) && ok;
		}

		protected  sealed override void DoEmit (EmitContext ec)
		{
			if (!inside_try_finally)
				EmitTryBodyPrepare (ec);

			Block.Emit (ec);

			LocalBuilder state_variable = null;
			foreach (Catch c in clauses) {
				c.Emit (ec);

				if (catch_sm != null) {
					if (state_variable == null) {
						//
						// Cannot reuse temp variable because non-catch path assumes the value is 0
						// which may not be true for reused local variable
						//
						state_variable = ec.DeclareLocal (ec.Module.Compiler.BuiltinTypes.Int, false);
					}

					var index = catch_sm.IndexOf (c);
					if (index < 0)
						continue;

					ec.EmitInt (index + 1);
					ec.Emit (OpCodes.Stloc, state_variable);
				}
			}

			if (!inside_try_finally)
				ec.EndExceptionBlock ();

			if (state_variable != null) {
				ec.Emit (OpCodes.Ldloc, state_variable);

				var labels = new Label [catch_sm.Count + 1];
				for (int i = 0; i < labels.Length; ++i) {
					labels [i] = ec.DefineLabel ();
				}

				var end = ec.DefineLabel ();
				ec.Emit (OpCodes.Switch, labels);

				// 0 value is default label
				ec.MarkLabel (labels [0]);
				ec.Emit (OpCodes.Br, end);

				var atv = ec.AsyncThrowVariable;
				Catch c = null;
				for (int i = 0; i < catch_sm.Count; ++i) {
					if (c != null && c.Block.HasReachableClosingBrace)
						ec.Emit (OpCodes.Br, end);

					ec.MarkLabel (labels [i + 1]);
					c = catch_sm [i];
					ec.AsyncThrowVariable = c.Variable;
					c.Block.Emit (ec);
				}
				ec.AsyncThrowVariable = atv;

				ec.MarkLabel (end);
			}
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			var start_fc = fc.BranchDefiniteAssignment ();
			var res = Block.FlowAnalysis (fc);

			DefiniteAssignmentBitSet try_fc = res ? null : fc.DefiniteAssignment;

			foreach (var c in clauses) {
				fc.BranchDefiniteAssignment (start_fc);
				if (!c.FlowAnalysis (fc)) {
					if (try_fc == null)
						try_fc = fc.DefiniteAssignment;
					else
						try_fc &= fc.DefiniteAssignment;

					res = false;
				}
			}

			fc.DefiniteAssignment = try_fc ?? start_fc;
			parent = null;
			return res;
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			if (rc.IsUnreachable)
				return rc;

			base.MarkReachable (rc);

			var tc_rc = Block.MarkReachable (rc);

			foreach (var c in clauses)
				tc_rc &= c.MarkReachable (rc);

			return tc_rc;
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			TryCatch target = (TryCatch) t;

			target.Block = clonectx.LookupBlock (Block);
			if (clauses != null){
				target.clauses = new List<Catch> ();
				foreach (Catch c in clauses)
					target.clauses.Add ((Catch) c.Clone (clonectx));
			}
		}

		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}

	

}