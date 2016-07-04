namespace VSC.AST {
public class Do : LoopStatement
	{
		/*public Expression expr;
		bool iterator_reachable, end_reachable;
       
		public Do (Statement statement, BooleanExpression bool_expr, Location doLocation, Location whileLocation)
			: base (statement)
		{
			expr = bool_expr;
			loc = doLocation;
			WhileLocation = whileLocation;
		}
    
		public Location WhileLocation {
			get; private set;
		}

		public override bool Resolve (BlockContext bc)
		{
			var ok = base.Resolve (bc);

			expr = expr.Resolve (bc);

			return ok;
		}
		
		protected override void DoEmit (EmitContext ec)
		{
			Label loop = ec.DefineLabel ();
			Label old_begin = ec.LoopBegin;
			Label old_end = ec.LoopEnd;
			
			ec.LoopBegin = ec.DefineLabel ();
			ec.LoopEnd = ec.DefineLabel ();
				
			ec.MarkLabel (loop);
			Statement.Emit (ec);
			ec.MarkLabel (ec.LoopBegin);

			// Mark start of while condition
			ec.Mark (WhileLocation);

			//
			// Dead code elimination
			//
			if (expr is Constant) {
				bool res = !((Constant) expr).IsDefaultValue;

				expr.EmitSideEffect (ec);
				if (res)
					ec.Emit (OpCodes.Br, loop);
			} else {
				expr.EmitBranchable (ec, loop, true);
			}
			
			ec.MarkLabel (ec.LoopEnd);

			ec.LoopBegin = old_begin;
			ec.LoopEnd = old_end;
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			var res = Statement.FlowAnalysis (fc);
            if (ElseStatement != null) 
                res &= ElseStatement.FlowAnalysis(fc);
			expr.FlowAnalysisConditional (fc);

			fc.DefiniteAssignment = fc.DefiniteAssignmentOnFalse;

			if (res && !iterator_reachable)
				return !end_reachable;

			if (!end_reachable) {
				var c = expr as Constant;
				if (c != null && !c.IsDefaultValue)
					return true;
			}

			return false;
		}
		
		public override Reachability MarkReachable (Reachability rc)
		{
			base.MarkReachable (rc);
            bool else_rc = false;
			var body_rc = Statement.MarkReachable (rc);
            if (ElseStatement != null)
                else_rc =  (ElseStatement.MarkReachable(rc).IsUnreachable);

			if (body_rc.IsUnreachable && else_rc && !iterator_reachable) {
				expr = new UnreachableExpression (expr);
				return end_reachable ? rc : Reachability.CreateUnreachable ();
			}

			if (!end_reachable) {
				var c = expr as Constant;
				if (c != null && !c.IsDefaultValue)
					return Reachability.CreateUnreachable ();
			}

			return rc;
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			Do target = (Do) t;

			target.Statement = Statement.Clone (clonectx);
			target.expr = expr.Clone (clonectx);
          if (ElseStatement != null)
              target.ElseStatement = ElseStatement.Clone(clonectx);
		}
		
		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
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