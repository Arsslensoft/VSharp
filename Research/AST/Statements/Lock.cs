namespace VSC.AST {
public class Lock : TryFinallyBlock
	{
		/*Expression expr;
		TemporaryVariableReference expr_copy;
		TemporaryVariableReference lock_taken;
			
		public Lock (Expression expr, Statement stmt, Location loc)
			: base (stmt, loc)
		{
			this.expr = expr;
		}

		public Expression Expr {
			get {
 				return this.expr;
			}
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			expr.FlowAnalysis (fc);
			return base.DoFlowAnalysis (fc);
		}

		public override bool Resolve (BlockContext ec)
		{
			expr = expr.Resolve (ec);
			if (expr == null)
				return false;

			if (!TypeSpec.IsReferenceType (expr.Type) && expr.Type != InternalType.ErrorType) {
				ec.Report.Error (185, loc,
					"`{0}' is not a reference type as required by the lock statement",
					expr.Type.GetSignatureForError ());
			}

			if (expr.Type.IsGenericParameter) {
				expr = Convert.ImplicitTypeParameterConversion (expr, (TypeParameterSpec)expr.Type, ec.BuiltinTypes.Object);
			}

			VariableReference lv = expr as VariableReference;
			bool locked;
			if (lv != null) {
				locked = lv.IsLockedByStatement;
				lv.IsLockedByStatement = true;
			} else {
				lv = null;
				locked = false;
			}

			//
			// Have to keep original lock value around to unlock same location
			// in the case of original value has changed or is null
			//
			expr_copy = TemporaryVariableReference.Create (ec.BuiltinTypes.Object, ec.CurrentBlock, loc);
			expr_copy.Resolve (ec);

			//
			// Ensure Monitor methods are available
			//
			if (ResolvePredefinedMethods (ec) > 1) {
				lock_taken = TemporaryVariableReference.Create (ec.BuiltinTypes.Bool, ec.CurrentBlock, loc);
				lock_taken.Resolve (ec);
			}

			using (ec.Set (ResolveContext.Options.LockScope)) {
				base.Resolve (ec);
			}

			if (lv != null) {
				lv.IsLockedByStatement = locked;
			}

			return true;
		}
		
		protected override void EmitTryBodyPrepare (EmitContext ec)
		{
			expr_copy.EmitAssign (ec, expr);

			if (lock_taken != null) {
				//
				// Initialize ref variable
				//
				lock_taken.EmitAssign (ec, new BoolLiteral (ec.BuiltinTypes, false, loc));
			} else {
				//
				// Monitor.Enter (expr_copy)
				//
				expr_copy.Emit (ec);
				ec.Emit (OpCodes.Call, ec.Module.PredefinedMembers.MonitorEnter.Get ());
			}

			base.EmitTryBodyPrepare (ec);
		}

		protected override void EmitTryBody (EmitContext ec)
		{
			//
			// Monitor.Enter (expr_copy, ref lock_taken)
			//
			if (lock_taken != null) {
				expr_copy.Emit (ec);
				lock_taken.LocalInfo.CreateBuilder (ec);
				lock_taken.AddressOf (ec, AddressOp.Load);
				ec.Emit (OpCodes.Call, ec.Module.PredefinedMembers.MonitorEnter_v4.Get ());
			}

			Statement.Emit (ec);
		}

		public override void EmitFinallyBody (EmitContext ec)
		{
			//
			// if (lock_taken) Monitor.Exit (expr_copy)
			//
			Label skip = ec.DefineLabel ();

			if (lock_taken != null) {
				lock_taken.Emit (ec);
				ec.Emit (OpCodes.Brfalse_S, skip);
			}

			expr_copy.Emit (ec);
			var m = ec.Module.PredefinedMembers.MonitorExit.Resolve (loc);
			if (m != null)
				ec.Emit (OpCodes.Call, m);

			ec.MarkLabel (skip);
		}

		int ResolvePredefinedMethods (ResolveContext rc)
		{
			// Try 4.0 Monitor.Enter (object, ref bool) overload first
			var m = rc.Module.PredefinedMembers.MonitorEnter_v4.Get ();
			if (m != null)
				return 4;

			m = rc.Module.PredefinedMembers.MonitorEnter.Get ();
			if (m != null)
				return 1;

			rc.Module.PredefinedMembers.MonitorEnter_v4.Resolve (loc);
			return 0;
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			Lock target = (Lock) t;

			target.expr = expr.Clone (clonectx);
			target.stmt = Statement.Clone (clonectx);
		}
		
		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}

*/	}
    

}