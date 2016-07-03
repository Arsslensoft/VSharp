namespace VSC.AST {

// 
	// Fixed statement
	//
	public class Fixed : Statement
	{
		/*abstract class Emitter : ShimExpression
		{
			protected LocalVariable vi;

			protected Emitter (Expression expr, LocalVariable li)
				: base (expr)
			{
				vi = li;
			}

			public abstract void EmitExit (EmitContext ec);

			public override void FlowAnalysis (FlowAnalysisContext fc)
			{
				expr.FlowAnalysis (fc);
			}
		}

		sealed class ExpressionEmitter : Emitter {
			public ExpressionEmitter (Expression converted, LocalVariable li)
				: base (converted, li)
			{
			}

			protected override Expression DoResolve (ResolveContext rc)
			{
				throw new NotImplementedException ();
			}

			public override void Emit (EmitContext ec) {
				//
				// Store pointer in pinned location
				//
				expr.Emit (ec);
				vi.EmitAssign (ec);
			}

			public override void EmitExit (EmitContext ec)
			{
				ec.EmitInt (0);
				ec.Emit (OpCodes.Conv_U);
				vi.EmitAssign (ec);
			}
		}

		class StringEmitter : Emitter
		{
			LocalVariable pinned_string;

			public StringEmitter (Expression expr, LocalVariable li)
				: base (expr, li)
			{
			}

			protected override Expression DoResolve (ResolveContext rc)
			{
				pinned_string = new LocalVariable (vi.Block, "$pinned",
					LocalVariable.Flags.FixedVariable | LocalVariable.Flags.CompilerGenerated | LocalVariable.Flags.Used,
					vi.Location);
				pinned_string.Type = rc.BuiltinTypes.String;
				vi.IsFixed = false;

				eclass = ExprClass.Variable;
				type = rc.BuiltinTypes.Int;
				return this;
			}

			public override void Emit (EmitContext ec)
			{
				pinned_string.CreateBuilder (ec);

				expr.Emit (ec);
				pinned_string.EmitAssign (ec);

				// TODO: Should use Binary::Addition
				pinned_string.Emit (ec);
				ec.Emit (OpCodes.Conv_I);

				var m = ec.Module.PredefinedMembers.RuntimeHelpersOffsetToStringData.Resolve (loc);
				if (m == null)
					return;

				PropertyExpr pe = new PropertyExpr (m, pinned_string.Location);
				//pe.InstanceExpression = pinned_string;
				pe.Resolve (new ResolveContext (ec.MemberContext)).Emit (ec);

				ec.Emit (OpCodes.Addition);
				vi.EmitAssign (ec);
			}

			public override void EmitExit (EmitContext ec)
			{
				ec.EmitNull ();
				pinned_string.EmitAssign (ec);
			}
		}

		public class VariableDeclaration : BlockVariable
		{
			public VariableDeclaration (FullNamedExpression type, LocalVariable li)
				: base (type, li)
			{
			}

			protected override Expression ResolveInitializer (BlockContext bc, LocalVariable li, Expression initializer)
			{
				if (!Variable.Type.IsPointer && li == Variable) {
					bc.Report.Error (209, TypeExpression.Location,
						"The type of locals declared in a fixed statement must be a pointer type");
					return null;
				}

				var res = initializer.Resolve (bc);
				if (res == null)
					return null;

				//
				// Case 1: Array
				//
				var ac = res.Type as ArrayContainer;
				if (ac != null) {
					TypeSpec array_type = ac.Element;

					//
					// Provided that array_type is unmanaged,
					//
					if (!TypeManager.VerifyUnmanaged (bc.Module, array_type, loc))
						return null;

					Expression res_init;
					if (ExpressionAnalyzer.IsInexpensiveLoad (res)) {
						res_init = res;
					} else {
						var expr_variable = LocalVariable.CreateCompilerGenerated (ac, bc.CurrentBlock, loc);
						res_init = new CompilerAssign (expr_variable.CreateReferenceExpression (bc, loc), res, loc);
						res = expr_variable.CreateReferenceExpression (bc, loc);
					}

					//
					// and T* is implicitly convertible to the
					// pointer type given in the fixed statement.
					//
					ArrayPtr array_ptr = new ArrayPtr (res, array_type, loc);

					Expression converted = Convert.ImplicitConversionRequired (bc, array_ptr.Resolve (bc), li.Type, loc);
					if (converted == null)
						return null;

					//
					// fixed (T* e_ptr = (e == null || e.Length == 0) ? null : converted [0])
					//
					converted = new Conditional (new BooleanExpression (new Binary (Binary.Operator.LogicalOr,
						new Binary (Binary.Operator.Equality, res_init, new NullLiteral (loc)),
						new Binary (Binary.Operator.Equality, new MemberAccess (res, "Length"), new IntConstant (bc.BuiltinTypes, 0, loc)))),
							new NullLiteral (loc),
							converted, loc);

					converted = converted.Resolve (bc);

					return new ExpressionEmitter (converted, li);
				}

				//
				// Case 2: string
				//
				if (res.Type.BuiltinType == BuiltinTypeSpec.Type.String) {
					return new StringEmitter (res, li).Resolve (bc);
				}

				// Case 3: fixed buffer
				if (res is FixedBufferPtr) {
					return new ExpressionEmitter (res, li);
				}

				bool already_fixed = true;

				//
				// Case 4: & object.
				//
				Unary u = res as Unary;
				if (u != null) {
					if (u.Oper == Unary.Operator.AddressOf) {
						IVariableReference vr = u.Expr as IVariableReference;
						if (vr == null || !vr.IsFixed) {
							already_fixed = false;
						}
					}
				} else if (initializer is Cast) {
					bc.Report.Error (254, initializer.Location, "The right hand side of a fixed statement assignment may not be a cast expression");
					return null;
				}

				if (already_fixed) {
					bc.Report.Error (213, loc, "You cannot use the fixed statement to take the address of an already fixed expression");
				}

				res = Convert.ImplicitConversionRequired (bc, res, li.Type, loc);
				return new ExpressionEmitter (res, li);
			}
		}


		VariableDeclaration decl;
		Statement statement;
		bool has_ret;

		public Fixed (VariableDeclaration decl, Statement stmt, Location l)
		{
			this.decl = decl;
			statement = stmt;
			loc = l;
		}

		#region Properties

		public Statement Statement {
			get {
				return statement;
			}
		}

		public BlockVariable Variables {
			get {
				return decl;
			}
		}

		#endregion

		public override bool Resolve (BlockContext bc)
		{
			using (bc.Set (ResolveContext.Options.FixedInitializerScope)) {
				if (!decl.Resolve (bc))
					return false;
			}

			return statement.Resolve (bc);
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			decl.FlowAnalysis (fc);
			return statement.FlowAnalysis (fc);
		}
		
		protected override void DoEmit (EmitContext ec)
		{
			decl.Variable.CreateBuilder (ec);
			decl.Initializer.Emit (ec);
			if (decl.Declarators != null) {
				foreach (var d in decl.Declarators) {
					d.Variable.CreateBuilder (ec);
					d.Initializer.Emit (ec);
				}
			}

			statement.Emit (ec);

			if (has_ret)
				return;

			//
			// Clear the pinned variable
			//
			((Emitter) decl.Initializer).EmitExit (ec);
			if (decl.Declarators != null) {
				foreach (var d in decl.Declarators) {
					((Emitter)d.Initializer).EmitExit (ec);
				}
			}
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			base.MarkReachable (rc);

			decl.MarkReachable (rc);

			rc = statement.MarkReachable (rc);

			// TODO: What if there is local exit?
			has_ret = rc.IsUnreachable;
			return rc;
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			Fixed target = (Fixed) t;

			target.decl = (VariableDeclaration) decl.Clone (clonectx);
			target.statement = statement.Clone (clonectx);
		}
		
		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}

	
}