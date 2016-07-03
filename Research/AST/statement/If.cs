namespace VSC.AST {
	public class If : Statement {
/*		Expression expr;
		public Statement TrueStatement;
		public Statement FalseStatement;

		bool true_returns, false_returns;

		public If (Expression bool_expr, Statement true_statement, Location l)
			: this (bool_expr, true_statement, null, l)
		{
		}

		public If (Expression bool_expr,
			   Statement true_statement,
			   Statement false_statement,
			   Location l)
		{
			this.expr = bool_expr;
			TrueStatement = true_statement;
			FalseStatement = false_statement;
			loc = l;
		}

		public Expression Expr {
			get {
				return this.expr;
			}
		}
		
		public override bool Resolve (BlockContext ec)
		{
			expr = expr.Resolve (ec);

			var ok = TrueStatement.Resolve (ec);

			if (FalseStatement != null) {
				ok &= FalseStatement.Resolve (ec);
			}

			return ok;
		}
		
		protected override void DoEmit (EmitContext ec)
		{
			Label false_target = ec.DefineLabel ();
			Label end;

			//
			// If we're a boolean constant, Resolve() already
			// eliminated dead code for us.
			//
			Constant c = expr as Constant;
			if (c != null){
				c.EmitSideEffect (ec);

				if (!c.IsDefaultValue)
					TrueStatement.Emit (ec);
				else if (FalseStatement != null)
					FalseStatement.Emit (ec);

				return;
			}			
			
			expr.EmitBranchable (ec, false_target, false);
			
			TrueStatement.Emit (ec);

			if (FalseStatement != null){
				bool branch_emitted = false;
				
				end = ec.DefineLabel ();
				if (!true_returns){
					ec.Emit (OpCodes.Br, end);
					branch_emitted = true;
				}

				ec.MarkLabel (false_target);
				FalseStatement.Emit (ec);

				if (branch_emitted)
					ec.MarkLabel (end);
			} else {
				ec.MarkLabel (false_target);
			}
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			expr.FlowAnalysisConditional (fc);

			var da_false = new DefiniteAssignmentBitSet (fc.DefiniteAssignmentOnFalse);

			fc.DefiniteAssignment = fc.DefiniteAssignmentOnTrue;

			var res = TrueStatement.FlowAnalysis (fc);

			if (FalseStatement == null) {
				var c = expr as Constant;
				if (c != null && !c.IsDefaultValue)
					return true_returns;

				if (true_returns)
					fc.DefiniteAssignment = da_false;
				else
					fc.DefiniteAssignment &= da_false;
 
				return false;
			}

			if (true_returns) {
				fc.DefiniteAssignment = da_false;
				return FalseStatement.FlowAnalysis (fc);
			}

			var da_true = fc.DefiniteAssignment;

			fc.DefiniteAssignment = da_false;
			res &= FalseStatement.FlowAnalysis (fc);

			if (!TrueStatement.IsUnreachable) {
				if (false_returns || FalseStatement.IsUnreachable)
					fc.DefiniteAssignment = da_true;
				else
					fc.DefiniteAssignment &= da_true;
			}

			return res;
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			if (rc.IsUnreachable)
				return rc;

			base.MarkReachable (rc);

			var c = expr as Constant;
			if (c != null) {
				bool take = !c.IsDefaultValue;
				if (take) {
					rc = TrueStatement.MarkReachable (rc);
				} else {
					if (FalseStatement != null)
						rc = FalseStatement.MarkReachable (rc);
				}

				return rc;
			}

			var true_rc = TrueStatement.MarkReachable (rc);
			true_returns = true_rc.IsUnreachable;
	
			if (FalseStatement == null)
				return rc;

			var false_rc = FalseStatement.MarkReachable (rc);
			false_returns = false_rc.IsUnreachable;

			return true_rc & false_rc;
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			If target = (If) t;

			target.expr = expr.Clone (clonectx);
			target.TrueStatement = TrueStatement.Clone (clonectx);
			if (FalseStatement != null)
				target.FalseStatement = FalseStatement.Clone (clonectx);
		}
		
		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}


}