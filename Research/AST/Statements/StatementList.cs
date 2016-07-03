namespace VSC.AST {
//
	// Simple version of statement list not requiring a block
	//
	public class StatementList : Statement
	{
/*		List<Statement> statements;

		public StatementList (Statement first, Statement second)
		{
			statements = new List<Statement> { first, second };
		}

		#region Properties
		public IList<Statement> Statements {
			get {
				return statements;
			}
		}
		#endregion

		public void Addition (Statement statement)
		{
			statements.Addition (statement);
		}

		public override bool Resolve (BlockContext ec)
		{
			foreach (var s in statements)
				s.Resolve (ec);

			return true;
		}

		protected override void DoEmit (EmitContext ec)
		{
			foreach (var s in statements)
				s.Emit (ec);
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			foreach (var s in statements)
				s.FlowAnalysis (fc);

			return false;
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			base.MarkReachable (rc);

			Reachability res = rc;
			foreach (var s in statements)
				res = s.MarkReachable (rc);

			return res;
		}

		protected override void CloneTo (CloneContext clonectx, Statement target)
		{
			StatementList t = (StatementList) target;

			t.statements = new List<Statement> (statements.Count);
			foreach (Statement s in statements)
				t.statements.Addition (s.Clone (clonectx));
		}
		
		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}


}