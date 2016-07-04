namespace VSC.AST {
public class StatementExpression : Statement
	{
/*		ExpressionStatement expr;
		
		public StatementExpression (ExpressionStatement expr)
		{
			this.expr = expr;
			loc = expr.StartLocation;
		}

		public StatementExpression (ExpressionStatement expr, Location loc)
		{
			this.expr = expr;
			this.loc = loc;
		}

		public ExpressionStatement Expr {
			get {
 				return this.expr;
			}
		}
		
		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			StatementExpression target = (StatementExpression) t;
			target.expr = (ExpressionStatement) expr.Clone (clonectx);
		}
		
		protected override void DoEmit (EmitContext ec)
		{
			expr.EmitStatement (ec);
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			expr.FlowAnalysis (fc);
			return false;
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			base.MarkReachable (rc);
			expr.MarkReachable (rc);
			return rc;
		}

		public override bool Resolve (BlockContext ec)
		{
			expr = expr.ResolveStatement (ec);
			return expr != null;
		}
		
		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}


}