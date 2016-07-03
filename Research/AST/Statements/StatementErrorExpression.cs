namespace VSC.AST {
public class StatementErrorExpression : Statement
	{
/*		Expression expr;

		public StatementErrorExpression (Expression expr)
		{
			this.expr = expr;
			this.loc = expr.StartLocation;
		}

		public Expression Expr {
			get {
				return expr;
			}
		}

		public override bool Resolve (BlockContext bc)
		{
			expr.Error_InvalidExpressionStatement (bc);
			return true;
		}

		protected override void DoEmit (EmitContext ec)
		{
			throw new NotSupportedException ();
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			return false;
		}

		protected override void CloneTo (CloneContext clonectx, Statement target)
		{
			var t = (StatementErrorExpression) target;

			t.expr = expr.Clone (clonectx);
		}
		
		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}



}