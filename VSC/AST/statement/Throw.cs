namespace VSC.AST {
public class Throw : Statement {
		/*Expression expr;
		
		public Throw (Expression expr, Location l)
		{
			this.expr = expr;
			loc = l;
		}

		public Expression Expr {
			get {
 				return this.expr;
			}
		}

		public override bool Resolve (BlockContext ec)
		{
			if (expr == null) {
				if (!ec.HasSet (ResolveContext.Options.CatchScope)) {
					ec.Report.Error (156, loc, "A throw statement with no arguments is not allowed outside of a catch clause");
				} else if (ec.HasSet (ResolveContext.Options.FinallyScope)) {
					for (var b = ec.CurrentBlock; b != null && !b.IsCatchBlock; b = b.Parent) {
						if (b.IsFinallyBlock) {
							ec.Report.Error (724, loc,
								"A throw statement with no arguments is not allowed inside of a finally clause nested inside of the innermost catch clause");
							break;
						}
					}
				}

				return true;
			}

			expr = expr.Resolve (ec, ResolveFlags.Type | ResolveFlags.VariableOrValue);

			if (expr == null)
				return false;

			var et = ec.BuiltinTypes.Exception;
			if (Convert.ImplicitConversionExists (ec, expr, et))
				expr = Convert.ImplicitConversion (ec, expr, et, loc);
			else
				ec.Report.Error (155, expr.Location, "The type caught or thrown must be derived from System.Exception");

			return true;
		}
			
		protected override void DoEmit (EmitContext ec)
		{
			if (expr == null) {
				var atv = ec.AsyncThrowVariable;
				if (atv != null) {
					if (atv.HoistedVariant != null) {
						atv.HoistedVariant.Emit (ec);
					} else {
						atv.Emit (ec);
					}

					ec.Emit (OpCodes.Throw);
				} else {
					ec.Emit (OpCodes.Rethrow);
				}
			} else {
				expr.Emit (ec);

				ec.Emit (OpCodes.Throw);
			}
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			if (expr != null)
				expr.FlowAnalysis (fc);

			return true;
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			base.MarkReachable (rc);
			return Reachability.CreateUnreachable ();
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			Throw target = (Throw) t;

			if (expr != null)
				target.expr = expr.Clone (clonectx);
		}
		
		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}



}