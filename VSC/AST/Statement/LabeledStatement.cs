namespace VSC.AST {
public class LabeledStatement : Statement {
		/*string name;
		bool defined;
		bool referenced;
		Label label;
		Block block;
		
		public LabeledStatement (string name, Block block, Location l)
		{
			this.name = name;
			this.block = block;
			this.loc = l;
		}

		public Label LabelTarget (EmitContext ec)
		{
			if (defined)
				return label;

			label = ec.DefineLabel ();
			defined = true;
			return label;
		}

		public Block Block {
			get {
				return block;
			}
		}

		public string Name {
			get { return name; }
		}

		protected override void CloneTo (CloneContext clonectx, Statement target)
		{
			var t = (LabeledStatement) target;

			t.block = clonectx.RemapBlockCopy (block);
		}

		public override bool Resolve (BlockContext bc)
		{
			return true;
		}

		protected override void DoEmit (EmitContext ec)
		{
			LabelTarget (ec);
			ec.MarkLabel (label);
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			if (!referenced) {
				fc.Report.Warning (164, 2, loc, "This label has not been referenced");
			}

			return false;
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			base.MarkReachable (rc);

			if (referenced)
				rc = new Reachability ();

			return rc;
		}

		public void AddGotoReference (Reachability rc)
		{
			if (referenced)
				return;

			referenced = true;
			MarkReachable (rc);

			block.ScanGotoJump (this);
		}

		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}
	

}