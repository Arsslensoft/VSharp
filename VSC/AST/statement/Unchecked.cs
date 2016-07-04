namespace VSC.AST {

	public class Unchecked : Statement {
		/*public Block Block;
		
		public Unchecked (Block b, Location loc)
		{
			Block = b;
			b.Unchecked = true;
			this.loc = loc;
		}

		public override bool Resolve (BlockContext ec)
		{
			using (ec.With (ResolveContext.Options.AllCheckStateFlags, false))
				return Block.Resolve (ec);
		}
		
		protected override void DoEmit (EmitContext ec)
		{
			using (ec.With (EmitContext.Options.CheckedScope, false))
				Block.Emit (ec);
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			return Block.FlowAnalysis (fc);
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			base.MarkReachable (rc);
			return Block.MarkReachable (rc);
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			Unchecked target = (Unchecked) t;

			target.Block = clonectx.LookupBlock (Block);
		}
		
		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}


}