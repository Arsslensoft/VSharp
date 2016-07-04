namespace VSC.AST {
	public class For : LoopStatement
	{
/*		bool infinite, empty, iterator_reachable, end_reachable;
		List<DefiniteAssignmentBitSet> end_reachable_das;
		
		public For (Location l)
			: base (null)
		{
			loc = l;
		}

		public Statement Initializer {
			get; set;
		}

		public Expression Condition {
			get; set;
		}

		public Statement Iterator {
			get; set;
		}

		public override bool Resolve (BlockContext bc)
		{
			Initializer.Resolve (bc);

			if (Condition != null) {
				Condition = Condition.Resolve (bc);
				var condition_constant = Condition as Constant;
				if (condition_constant != null) {
					if (condition_constant.IsDefaultValue) {
						empty = true;
					} else {
						infinite = true;
					}
				}
			} else {
				infinite = true;
			}

			return base.Resolve (bc) && Iterator.Resolve (bc);
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			Initializer.FlowAnalysis (fc);

			DefiniteAssignmentBitSet da_false;
			if (Condition != null) {
				Condition.FlowAnalysisConditional (fc);
				fc.DefiniteAssignment = fc.DefiniteAssignmentOnTrue;
				da_false = new DefiniteAssignmentBitSet (fc.DefiniteAssignmentOnFalse);
			} else {
				da_false = fc.BranchDefiniteAssignment ();
			}

			Statement.FlowAnalysis (fc);

			Iterator.FlowAnalysis (fc);

			//
			// Special case infinite for with breaks
			//
			if (end_reachable_das != null) {
				da_false = DefiniteAssignmentBitSet.And (end_reachable_das);
				end_reachable_das = null;
			}

			fc.DefiniteAssignment = da_false;

			if (infinite && !end_reachable)
				return true;

			return false;
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			base.MarkReachable (rc);

			Initializer.MarkReachable (rc);

			var body_rc = Statement.MarkReachable (rc);
			if (!body_rc.IsUnreachable || iterator_reachable) {
				Iterator.MarkReachable (rc);
			}

			//
			// When infinite for end is unreachable via break anything what follows is unreachable too
			//
			if (infinite && !end_reachable) {
				return Reachability.CreateUnreachable ();
			}

			return rc;
		}

		protected override void DoEmit (EmitContext ec)
		{
			if (Initializer != null)
				Initializer.Emit (ec);

			if (empty) {
				Condition.EmitSideEffect (ec);
				return;
			}

			Label old_begin = ec.LoopBegin;
			Label old_end = ec.LoopEnd;
			Label loop = ec.DefineLabel ();
			Label test = ec.DefineLabel ();

			ec.LoopBegin = ec.DefineLabel ();
			ec.LoopEnd = ec.DefineLabel ();

			ec.Emit (OpCodes.Br, test);
			ec.MarkLabel (loop);
			Statement.Emit (ec);

			ec.MarkLabel (ec.LoopBegin);
			Iterator.Emit (ec);

			ec.MarkLabel (test);
			//
			// If test is null, there is no test, and we are just
			// an infinite loop
			//
			if (Condition != null) {
				ec.Mark (Condition.Location);

				//
				// The Resolve code already catches the case for
				// Test == Constant (false) so we know that
				// this is true
				//
				if (Condition is Constant) {
					Condition.EmitSideEffect (ec);
					ec.Emit (OpCodes.Br, loop);
				} else {
					Condition.EmitBranchable (ec, loop, true);
				}
				
			} else
				ec.Emit (OpCodes.Br, loop);
			ec.MarkLabel (ec.LoopEnd);

			ec.LoopBegin = old_begin;
			ec.LoopEnd = old_end;
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			For target = (For) t;

			if (Initializer != null)
				target.Initializer = Initializer.Clone (clonectx);
			if (Condition != null)
				target.Condition = Condition.Clone (clonectx);
			if (Iterator != null)
				target.Iterator = Iterator.Clone (clonectx);
			target.Statement = Statement.Clone (clonectx);
		}

		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}

		public override void AddEndDefiniteAssignment (FlowAnalysisContext fc)
		{
			if (!infinite)
				return;

			if (end_reachable_das == null)
				end_reachable_das = new List<DefiniteAssignmentBitSet> ();

			end_reachable_das.Add (fc.DefiniteAssignment);
		}

		public override void SetEndReachable ()
		{
			end_reachable = true;
		}

		public override void SetIteratorReachable ()
		{
			iterator_reachable = true;
		}*/
	}


}