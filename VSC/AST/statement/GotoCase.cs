namespace VSC.AST {
/// <summary>
	///   `goto case' statement
	/// </summary>
	public class GotoCase : SwitchGoto
	{
		/*Expression expr;
		
		public GotoCase (Expression e, Location l)
			: base (l)
		{
			expr = e;
		}

		public Expression Expr {
			get {
 				return expr;
			}
		}

		public SwitchLabel Label { get; set; }

		public override bool Resolve (BlockContext ec)
		{
			if (ec.Switch == null) {
				Error_GotoCaseRequiresSwitchBlock (ec);
				return false;
			}

			Constant c = expr.ResolveLabelConstant (ec);
			if (c == null) {
				return false;
			}

			Constant res;
			if (ec.Switch.IsNullable && c is NullLiteral) {
				res = c;
			} else {
				TypeSpec type = ec.Switch.SwitchType;
				res = c.Reduce (ec, type);
				if (res == null) {
					c.Error_ValueCannotBeConverted (ec, type, true);
					return false;
				}

				if (!Convert.ImplicitStandardConversionExists (c, type))
					ec.Report.Warning (469, 2, loc,
						"The `goto case' value is not implicitly convertible to type `{0}'",
						type.GetSignatureForError ());

			}

			ec.Switch.RegisterGotoCase (this, res);
			base.Resolve (ec);
			expr = res;

			return true;
		}

		protected override void DoEmit (EmitContext ec)
		{
			ec.Emit (unwind_protect ? OpCodes.Leave : OpCodes.Br, Label.GetILLabel (ec));
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			GotoCase target = (GotoCase) t;

			target.expr = expr.Clone (clonectx);
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			if (!rc.IsUnreachable) {
				var label = switch_statement.FindLabel ((Constant) expr);
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