namespace VSC.AST {
/// <summary>
	///   `goto default' statement
	/// </summary>
	public class GotoDefault : SwitchGoto
	{		
		/*public GotoDefault (Location l)
			: base (l)
		{
		}

		public override bool Resolve (BlockContext bc)
		{
			if (bc.Switch == null) {
				Error_GotoCaseRequiresSwitchBlock (bc);
				return false;
			}

			bc.Switch.RegisterGotoCase (null, null);
			base.Resolve (bc);

			return true;
		}

		protected override void DoEmit (EmitContext ec)
		{
			ec.Emit (unwind_protect ? OpCodes.Leave : OpCodes.Br, ec.Switch.DefaultLabel.GetILLabel (ec));
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			if (!rc.IsUnreachable) {
				var label = switch_statement.DefaultLabel;
				if (label.IsUnreachable) {
					label.MarkReachable (rc);
					switch_statement.Block.ScanGotoJump (label);
				}
			}

			return base.MarkReachable (rc);
		}

		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}


}