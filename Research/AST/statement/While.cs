namespace VSC.AST {

public class While : LoopStatement
	{
		/*public Expression expr;
		bool empty, infinite, end_reachable;
		List<DefiniteAssignmentBitSet> end_reachable_das;
    
		public While (BooleanExpression bool_expr, Statement statement, Location l)
			: base (statement)
		{
			this.expr = bool_expr;
			loc = l;
		}

        public While(BooleanExpression bool_expr, Statement statement,Statement elsestmt,Location el, Location l)
            : base(statement,elsestmt)
        {
            this.expr = bool_expr;
            loc = l;
            ElseLocation = el;
        }
		public override bool Resolve (BlockContext bc)
		{
			bool ok = true;

			expr = expr.Resolve (bc);
			if (expr == null)
				ok = false;

			var c = expr as Constant;
			if (c != null) {
				empty = c.IsDefaultValue;
				infinite = !empty;
			}

			ok &= base.Resolve (bc);
			return ok;
		}
		
		protected override void DoEmit (EmitContext ec)
		{
            if (empty)
            {

                if (ElseStatement == null)
                {
                    expr.EmitSideEffect(ec);
                    return;
                }
                else
                {
                    ElseStatement.Emit(ec);
                    return;
                }
			}

			Label old_begin = ec.LoopBegin;
			Label old_end = ec.LoopEnd;
        
           
           
          
         

			ec.LoopBegin = ec.DefineLabel ();
			ec.LoopEnd = ec.DefineLabel ();

			//
			// Inform whether we are infinite or not
			//
			if (expr is Constant) {
				// expr is 'true', since the 'empty' case above handles the 'false' case
				ec.MarkLabel (ec.LoopBegin);

				if (ec.EmitAccurateDebugInfo)
					ec.Emit (OpCodes.Nop);

				expr.EmitSideEffect (ec);
				Statement.Emit (ec);
				ec.Emit (OpCodes.Br, ec.LoopBegin);
					
				//
				// Inform that we are infinite (ie, `we return'), only
				// if we do not `break' inside the code.
				//
				ec.MarkLabel (ec.LoopEnd);
			} else {

               
				Label while_loop = ec.DefineLabel ();

                if (ElseStatement != null)
                    EmitElse(ec, expr, while_loop, ElseLocation);

				ec.Emit (OpCodes.Br, ec.LoopBegin);
				ec.MarkLabel (while_loop);

				Statement.Emit (ec);
			
				ec.MarkLabel (ec.LoopBegin);

				ec.Mark (loc);
				expr.EmitBranchable (ec, while_loop, true);
				
				ec.MarkLabel (ec.LoopEnd);
			}	

			ec.LoopBegin = old_begin;
			ec.LoopEnd = old_end;
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			expr.FlowAnalysisConditional (fc);

			fc.DefiniteAssignment = fc.DefiniteAssignmentOnTrue;
			var da_false = new DefiniteAssignmentBitSet (fc.DefiniteAssignmentOnFalse);

			Statement.FlowAnalysis (fc);
            if (ElseStatement != null)
                ElseStatement.FlowAnalysis(fc);
			//
			// Special case infinite while with breaks
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
			if (rc.IsUnreachable)
				return rc;

			base.MarkReachable (rc);

			//
			// Special case unreachable while body
			//
			if (empty) {
				Statement.MarkReachable (Reachability.CreateUnreachable ());
				return rc;
			}

			Statement.MarkReachable (rc);
         
            if (ElseStatement != null)
                ElseStatement.MarkReachable(rc);
			//
			// When infinite while end is unreachable via break anything what follows is unreachable too
			//
			if (infinite && !end_reachable)
				return Reachability.CreateUnreachable ();

			return rc;
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			While target = (While) t;

			target.expr = expr.Clone (clonectx);
			target.Statement = Statement.Clone (clonectx);
            if (ElseStatement != null)
                target.ElseStatement = ElseStatement.Clone(clonectx);
         
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
		}*/
	}

}