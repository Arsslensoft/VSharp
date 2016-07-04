namespace VSC.AST {
public class Using : TryFinallyBlock
	{
		/*public class VariableDeclaration : BlockVariable
		{
			Statement dispose_call;

			public VariableDeclaration (FullNamedExpression type, LocalVariable li)
				: base (type, li)
			{
			}

			public VariableDeclaration (LocalVariable li, Location loc)
				: base (li)
			{
				reachable = true;
				this.loc = loc;
			}

			public VariableDeclaration (Expression expr)
				: base (null)
			{
				loc = expr.Location;
				Initializer = expr;
			}

			#region Properties

			public bool IsNested { get; private set; }

			#endregion

			public void EmitDispose (EmitContext ec)
			{
				dispose_call.Emit (ec);
			}

			public override bool Resolve (BlockContext bc)
			{
				if (IsNested)
					return true;

				return base.Resolve (bc, false);
			}

			public Expression ResolveExpression (BlockContext bc)
			{
				var e = Initializer.Resolve (bc);
				if (e == null)
					return null;

				li = LocalVariable.CreateCompilerGenerated (e.Type, bc.CurrentBlock, loc);
				Initializer = ResolveInitializer (bc, Variable, e);
				return e;
			}

			protected override Expression ResolveInitializer (BlockContext bc, LocalVariable li, Expression initializer)
			{
				if (li.Type.BuiltinType == BuiltinTypeSpec.Type.Dynamic) {
					initializer = initializer.Resolve (bc);
					if (initializer == null)
						return null;

					// Once there is dynamic used defer conversion to runtime even if we know it will never succeed
					Arguments args = new Arguments (1);
					args.Add (new Argument (initializer));
					initializer = new DynamicConversion (bc.BuiltinTypes.IDisposable, 0, args, initializer.Location).Resolve (bc);
					if (initializer == null)
						return null;

					var var = LocalVariable.CreateCompilerGenerated (initializer.Type, bc.CurrentBlock, loc);
					dispose_call = CreateDisposeCall (bc, var);
					dispose_call.Resolve (bc);

					return base.ResolveInitializer (bc, li, new SimpleAssign (var.CreateReferenceExpression (bc, loc), initializer, loc));
				}

				if (li == Variable) {
					CheckIDiposableConversion (bc, li, initializer);
					dispose_call = CreateDisposeCall (bc, li);
					dispose_call.Resolve (bc);
				}

				return base.ResolveInitializer (bc, li, initializer);
			}

			protected virtual void CheckIDiposableConversion (BlockContext bc, LocalVariable li, Expression initializer)
			{
				var type = li.Type;

				if (type.BuiltinType != BuiltinTypeSpec.Type.IDisposable && !CanConvertToIDisposable (bc, type)) {
					if (type.IsNullableType) {
						// it's handled in CreateDisposeCall
						return;
					}

					if (type != InternalType.ErrorType) {
						bc.Report.SymbolRelatedToPreviousError (type);
						var loc = type_expr == null ? initializer.Location : type_expr.Location;
						bc.Report.Error (1674, loc, "`{0}': type used in a using statement must be implicitly convertible to `System.IDisposable'",
							type.GetSignatureForError ());
					}

					return;
				}
			}

			static bool CanConvertToIDisposable (BlockContext bc, TypeSpec type)
			{
				var target = bc.BuiltinTypes.IDisposable;
				var tp = type as TypeParameterSpec;
				if (tp != null)
					return Convert.ImplicitTypeParameterConversion (null, tp, target) != null;

				return type.ImplementsInterface (target, false);
			}

			protected virtual Statement CreateDisposeCall (BlockContext bc, LocalVariable lv)
			{
				var lvr = lv.CreateReferenceExpression (bc, lv.Location);
				var type = lv.Type;
				var loc = lv.Location;

				var idt = bc.BuiltinTypes.IDisposable;
				var m = bc.Module.PredefinedMembers.IDisposableDispose.Resolve (loc);

				var dispose_mg = MethodGroupExpr.CreatePredefined (m, idt, loc);
				dispose_mg.InstanceExpression = type.IsNullableType ?
					new Cast (new TypeExpression (idt, loc), lvr, loc).Resolve (bc) :
					lvr;

				//
				// Hide it from symbol file via null location
				//
				Statement dispose = new StatementExpression (new Invocation (dispose_mg, null), Location.Null);

				// Add conditional call when disposing possible null variable
				if (!TypeSpec.IsValueType (type) || type.IsNullableType)
					dispose = new If (new Binary (Binary.Operator.Inequality, lvr, new NullLiteral (loc)), dispose, dispose.loc);

				return dispose;
			}

			public void ResolveDeclaratorInitializer (BlockContext bc)
			{
				Initializer = base.ResolveInitializer (bc, Variable, Initializer);
			}

			public Statement RewriteUsingDeclarators (BlockContext bc, Statement stmt)
			{
				for (int i = declarators.Count - 1; i >= 0; --i) {
					var d = declarators [i];
					var vd = new VariableDeclaration (d.Variable, d.Variable.Location);
					vd.Initializer = d.Initializer;
					vd.IsNested = true;
					vd.dispose_call = CreateDisposeCall (bc, d.Variable);
					vd.dispose_call.Resolve (bc);

					stmt = new Using (vd, stmt, d.Variable.Location);
				}

				declarators = null;
				return stmt;
			}	

			public override object Accept (StructuralVisitor visitor)
			{
				return visitor.Visit (this);
			}	
		}

		VariableDeclaration decl;

		public Using (VariableDeclaration decl, Statement stmt, Location loc)
			: base (stmt, loc)
		{
			this.decl = decl;
		}

		public Using (Expression expr, Statement stmt, Location loc)
			: base (stmt, loc)
		{
			this.decl = new VariableDeclaration (expr);
		}

		#region Properties

		public Expression Expr {
			get {
				return decl.Variable == null ? decl.Initializer : null;
			}
		}

		public BlockVariable Variables {
			get {
				return decl;
			}
		}

		#endregion

		public override void Emit (EmitContext ec)
		{
			//
			// Don't emit sequence point it will be set on variable declaration
			//
			DoEmit (ec);
		}

		protected override void EmitTryBodyPrepare (EmitContext ec)
		{
			decl.Emit (ec);
			base.EmitTryBodyPrepare (ec);
		}

		protected override void EmitTryBody (EmitContext ec)
		{
			stmt.Emit (ec);
		}

		public override void EmitFinallyBody (EmitContext ec)
		{
			decl.EmitDispose (ec);
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			decl.FlowAnalysis (fc);
			return stmt.FlowAnalysis (fc);
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			decl.MarkReachable (rc);
			return base.MarkReachable (rc);
		}

		public override bool Resolve (BlockContext ec)
		{
			VariableReference vr;
			bool vr_locked = false;

			using (ec.Set (ResolveContext.Options.UsingInitializerScope)) {
				if (decl.Variable == null) {
					vr = decl.ResolveExpression (ec) as VariableReference;
					if (vr != null) {
						vr_locked = vr.IsLockedByStatement;
						vr.IsLockedByStatement = true;
					}
				} else {
					if (decl.IsNested) {
						decl.ResolveDeclaratorInitializer (ec);
					} else {
						if (!decl.Resolve (ec))
							return false;

						if (decl.Declarators != null) {
							stmt = decl.RewriteUsingDeclarators (ec, stmt);
						}
					}

					vr = null;
				}
			}

			var ok = base.Resolve (ec);

			if (vr != null)
				vr.IsLockedByStatement = vr_locked;

			return ok;
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			Using target = (Using) t;

			target.decl = (VariableDeclaration) decl.Clone (clonectx);
			target.stmt = stmt.Clone (clonectx);
		}

		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}

	

}