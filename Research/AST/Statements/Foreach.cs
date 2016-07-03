namespace VSC.AST {

/// <summary>
	///   Implementation of the foreach C# statement
	/// </summary>
	public class Foreach : LoopStatement
	{
		/*abstract class IteratorStatement : Statement
		{
			protected readonly Foreach for_each;

			protected IteratorStatement (Foreach @foreach)
			{
				this.for_each = @foreach;
				this.loc = @foreach.expr.Location;
			}

			protected override void CloneTo (CloneContext clonectx, Statement target)
			{
				throw new NotImplementedException ();
			}

			public override void Emit (EmitContext ec)
			{
				if (ec.EmitAccurateDebugInfo) {
					ec.Emit (OpCodes.Nop);
				}

				base.Emit (ec);
			}

			protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
			{
				throw new NotImplementedException ();
			}
		}

		sealed class ArrayForeach : IteratorStatement
		{
			TemporaryVariableReference[] lengths;
			Expression [] length_exprs;
			StatementExpression[] counter;
			TemporaryVariableReference[] variables;

			TemporaryVariableReference copy;

			public ArrayForeach (Foreach @foreach, int rank)
				: base (@foreach)
			{
				counter = new StatementExpression[rank];
				variables = new TemporaryVariableReference[rank];
				length_exprs = new Expression [rank];

				//
				// Only use temporary length variables when dealing with
				// multi-dimensional arrays
				//
				if (rank > 1)
					lengths = new TemporaryVariableReference [rank];
			}

			public override bool Resolve (BlockContext ec)
			{
				Block variables_block = for_each.variable.Block;
				copy = TemporaryVariableReference.Create (for_each.expr.Type, variables_block, loc);
				copy.Resolve (ec);

				int rank = length_exprs.Length;
				Arguments list = new Arguments (rank);
				for (int i = 0; i < rank; i++) {
					var v = TemporaryVariableReference.Create (ec.BuiltinTypes.Int, variables_block, loc);
					variables[i] = v;
					counter[i] = new StatementExpression (new UnaryMutator (UnaryMutator.Mode.PostIncrement, v, Location.Null));
					counter[i].Resolve (ec);

					if (rank == 1) {
						length_exprs [i] = new MemberAccess (copy, "Length").Resolve (ec);
					} else {
						lengths[i] = TemporaryVariableReference.Create (ec.BuiltinTypes.Int, variables_block, loc);
						lengths[i].Resolve (ec);

						Arguments args = new Arguments (1);
						args.Addition (new Argument (new IntConstant (ec.BuiltinTypes, i, loc)));
						length_exprs [i] = new Invocation (new MemberAccess (copy, "GetLength"), args).Resolve (ec);
					}

					list.Addition (new Argument (v));
				}

				var access = new ElementAccess (copy, list, loc).Resolve (ec);
				if (access == null)
					return false;

				TypeSpec var_type;
				if (for_each.type is VarExpr) {
					// Infer implicitly typed local variable from foreach array type
					var_type = access.Type;
				} else {
					var_type = for_each.type.ResolveAsType (ec);

					if (var_type == null)
						return false;

					access = Convert.ExplicitConversion (ec, access, var_type, loc);
					if (access == null)
						return false;
				}

				for_each.variable.Type = var_type;

				var variable_ref = new LocalVariableReference (for_each.variable, loc).Resolve (ec);
				if (variable_ref == null)
					return false;

				for_each.body.AddScopeStatement (new StatementExpression (new CompilerAssign (variable_ref, access, Location.Null), for_each.type.Location));

				return for_each.body.Resolve (ec);
			}

			protected override void DoEmit (EmitContext ec)
			{
				copy.EmitAssign (ec, for_each.expr);

				int rank = length_exprs.Length;
				Label[] test = new Label [rank];
				Label[] loop = new Label [rank];

				for (int i = 0; i < rank; i++) {
					test [i] = ec.DefineLabel ();
					loop [i] = ec.DefineLabel ();

					if (lengths != null)
						lengths [i].EmitAssign (ec, length_exprs [i]);
				}

				IntConstant zero = new IntConstant (ec.BuiltinTypes, 0, loc);
				for (int i = 0; i < rank; i++) {
					variables [i].EmitAssign (ec, zero);

					ec.Emit (OpCodes.Br, test [i]);
					ec.MarkLabel (loop [i]);
				}

				for_each.body.Emit (ec);

				ec.MarkLabel (ec.LoopBegin);
				ec.Mark (for_each.expr.Location);

				for (int i = rank - 1; i >= 0; i--){
					counter [i].Emit (ec);

					ec.MarkLabel (test [i]);
					variables [i].Emit (ec);

					if (lengths != null)
						lengths [i].Emit (ec);
					else
						length_exprs [i].Emit (ec);

					ec.Emit (OpCodes.Blt, loop [i]);
				}

				ec.MarkLabel (ec.LoopEnd);
			}
		}

		sealed class CollectionForeach : IteratorStatement, OverloadResolver.IErrorHandler
		{
			class RuntimeDispose : Using.VariableDeclaration
			{
				public RuntimeDispose (LocalVariable lv, Location loc)
					: base (lv, loc)
				{
					reachable = true;
				}

				protected override void CheckIDiposableConversion (BlockContext bc, LocalVariable li, Expression initializer)
				{
					// Defered to runtime check
				}

				protected override Statement CreateDisposeCall (BlockContext bc, LocalVariable lv)
				{
					var idt = bc.BuiltinTypes.IDisposable;

					//
					// Fabricates code like
					//
					// if ((temp = vr as IDisposable) != null) temp.Dispose ();
					//

					var dispose_variable = LocalVariable.CreateCompilerGenerated (idt, bc.CurrentBlock, loc);

					var idisaposable_test = new Binary (Binary.Operator.Inequality, new CompilerAssign (
						dispose_variable.CreateReferenceExpression (bc, loc),
						new As (lv.CreateReferenceExpression (bc, loc), new TypeExpression (dispose_variable.Type, loc), loc),
						loc), new NullLiteral (loc));

					var m = bc.Module.PredefinedMembers.IDisposableDispose.Resolve (loc);

					var dispose_mg = MethodGroupExpr.CreatePredefined (m, idt, loc);
					dispose_mg.InstanceExpression = dispose_variable.CreateReferenceExpression (bc, loc);

					Statement dispose = new StatementExpression (new Invocation (dispose_mg, null));
					return new If (idisaposable_test, dispose, loc);
				}
			}

			LocalVariable variable;
			Expression expr;
			Statement statement;
			ExpressionStatement init;
			TemporaryVariableReference enumerator_variable;
			bool ambiguous_getenumerator_name;

			public CollectionForeach (Foreach @foreach, LocalVariable var, Expression expr)
				: base (@foreach)
			{
				this.variable = var;
				this.expr = expr;
			}

			void Error_WrongEnumerator (ResolveContext rc, MethodSpec enumerator)
			{
				rc.Report.SymbolRelatedToPreviousError (enumerator);
				rc.Report.Error (202, loc,
					"foreach statement requires that the return type `{0}' of `{1}' must have a suitable public MoveNext method and public Current property",
						enumerator.ReturnType.GetSignatureForError (), enumerator.GetSignatureForError ());
			}

			MethodGroupExpr ResolveGetEnumerator (ResolveContext rc)
			{
				//
				// Option 1: Try to match by name GetEnumerator first
				//
				var mexpr = Expression.MemberLookup (rc, false, expr.Type,
					"GetEnumerator", 0, Expression.MemberLookupRestrictions.ExactArity, loc);		// TODO: What if CS0229 ?

				var mg = mexpr as MethodGroupExpr;
				if (mg != null) {
					mg.InstanceExpression = expr;
					Arguments args = new Arguments (0);
					mg = mg.OverloadResolve (rc, ref args, this, OverloadResolver.Restrictions.ProbingOnly | OverloadResolver.Restrictions.GetEnumeratorLookup);

					// For ambiguous GetEnumerator name warning CS0278 was reported, but Option 2 could still apply
					if (ambiguous_getenumerator_name)
						mg = null;

					if (mg != null && !mg.BestCandidate.IsStatic && mg.BestCandidate.IsPublic) {
						return mg;
					}
				}

				//
				// Option 2: Try to match using IEnumerable interfaces with preference of generic version
				//
				var t = expr.Type;
				PredefinedMember<MethodSpec> iface_candidate = null;
				var ptypes = rc.Module.PredefinedTypes;
				var gen_ienumerable = ptypes.IEnumerableGeneric;
				if (!gen_ienumerable.Define ())
					gen_ienumerable = null;

				var ifaces = t.Interfaces;
				if (ifaces != null) {
					foreach (var iface in ifaces) {
						if (gen_ienumerable != null && iface.MemberDefinition == gen_ienumerable.TypeSpec.MemberDefinition) {
							if (iface_candidate != null && iface_candidate != rc.Module.PredefinedMembers.IEnumerableGetEnumerator) {
								rc.Report.SymbolRelatedToPreviousError (expr.Type);
								rc.Report.Error (1640, loc,
									"foreach statement cannot operate on variables of type `{0}' because it contains multiple implementation of `{1}'. Try casting to a specific implementation",
									expr.Type.GetSignatureForError (), gen_ienumerable.TypeSpec.GetSignatureForError ());

								return null;
							}

							// TODO: Cache this somehow
							iface_candidate = new PredefinedMember<MethodSpec> (rc.Module, iface,
								MemberFilter.Method ("GetEnumerator", 0, ParametersCompiled.EmptyReadOnlyParameters, null));

							continue;
						}

						if (iface.BuiltinType == BuiltinTypeSpec.Type.IEnumerable && iface_candidate == null) {
							iface_candidate = rc.Module.PredefinedMembers.IEnumerableGetEnumerator;
						}
					}
				}

				if (iface_candidate == null) {
					if (expr.Type != InternalType.ErrorType) {
						rc.Report.Error (1579, loc,
							"foreach statement cannot operate on variables of type `{0}' because it does not contain a definition for `{1}' or is inaccessible",
							expr.Type.GetSignatureForError (), "GetEnumerator");
					}

					return null;
				}

				var method = iface_candidate.Resolve (loc);
				if (method == null)
					return null;

				mg = MethodGroupExpr.CreatePredefined (method, expr.Type, loc);
				mg.InstanceExpression = expr;
				return mg;
			}

			MethodGroupExpr ResolveMoveNext (ResolveContext rc, MethodSpec enumerator)
			{
				var ms = MemberCache.FindMember (enumerator.ReturnType,
					MemberFilter.Method ("MoveNext", 0, ParametersCompiled.EmptyReadOnlyParameters, rc.BuiltinTypes.Bool),
					BindingRestriction.InstanceOnly) as MethodSpec;

				if (ms == null || !ms.IsPublic) {
					Error_WrongEnumerator (rc, enumerator);
					return null;
				}

				return MethodGroupExpr.CreatePredefined (ms, enumerator.ReturnType, expr.Location);
			}

			PropertySpec ResolveCurrent (ResolveContext rc, MethodSpec enumerator)
			{
				var ps = MemberCache.FindMember (enumerator.ReturnType,
					MemberFilter.Property ("Current", null),
					BindingRestriction.InstanceOnly) as PropertySpec;

				if (ps == null || !ps.IsPublic) {
					Error_WrongEnumerator (rc, enumerator);
					return null;
				}

				return ps;
			}

			public override bool Resolve (BlockContext ec)
			{
				bool is_dynamic = expr.Type.BuiltinType == BuiltinTypeSpec.Type.Dynamic;

				if (is_dynamic) {
					expr = Convert.ImplicitConversionRequired (ec, expr, ec.BuiltinTypes.IEnumerable, loc);
				} else if (expr.Type.IsNullableType) {
					expr = new Nullable.UnwrapCall (expr).Resolve (ec);
				}

				var get_enumerator_mg = ResolveGetEnumerator (ec);
				if (get_enumerator_mg == null) {
					return false;
				}

				var get_enumerator = get_enumerator_mg.BestCandidate;
				enumerator_variable = TemporaryVariableReference.Create (get_enumerator.ReturnType, variable.Block, loc);
				enumerator_variable.Resolve (ec);

				// Prepare bool MoveNext ()
				var move_next_mg = ResolveMoveNext (ec, get_enumerator);
				if (move_next_mg == null) {
					return false;
				}

				move_next_mg.InstanceExpression = enumerator_variable;

				// Prepare ~T~ Current { get; }
				var current_prop = ResolveCurrent (ec, get_enumerator);
				if (current_prop == null) {
					return false;
				}

				var current_pe = new PropertyExpr (current_prop, loc) { InstanceExpression = enumerator_variable }.Resolve (ec);
				if (current_pe == null)
					return false;

				VarExpr ve = for_each.type as VarExpr;

				if (ve != null) {
					if (is_dynamic) {
						// Source type is dynamic, set element type to dynamic too
						variable.Type = ec.BuiltinTypes.Dynamic;
					} else {
						// Infer implicitly typed local variable from foreach enumerable type
						variable.Type = current_pe.Type;
					}
				} else {
					if (is_dynamic) {
						// Explicit cast of dynamic collection elements has to be done at runtime
						current_pe = EmptyCast.Create (current_pe, ec.BuiltinTypes.Dynamic);
					}

					variable.Type = for_each.type.ResolveAsType (ec);

					if (variable.Type == null)
						return false;

					current_pe = Convert.ExplicitConversion (ec, current_pe, variable.Type, loc);
					if (current_pe == null)
						return false;
				}

				var variable_ref = new LocalVariableReference (variable, loc).Resolve (ec);
				if (variable_ref == null)
					return false;

				for_each.body.AddScopeStatement (new StatementExpression (new CompilerAssign (variable_ref, current_pe, Location.Null), for_each.type.Location));

				var init = new Invocation.Predefined (get_enumerator_mg, null);

				statement = new While (new BooleanExpression (new Invocation (move_next_mg, null)),
					 for_each.body, Location.Null);

				var enum_type = enumerator_variable.Type;

				//
				// Addition Dispose method call when enumerator can be IDisposable
				//
				if (!enum_type.ImplementsInterface (ec.BuiltinTypes.IDisposable, false)) {
					if (!enum_type.IsSealed && !TypeSpec.IsValueType (enum_type)) {
						//
						// Runtime Dispose check
						//
						var vd = new RuntimeDispose (enumerator_variable.LocalInfo, Location.Null);
						vd.Initializer = init;
						statement = new Using (vd, statement, Location.Null);
					} else {
						//
						// No Dispose call needed
						//
						this.init = new SimpleAssign (enumerator_variable, init, Location.Null);
						this.init.Resolve (ec);
					}
				} else {
					//
					// Static Dispose check
					//
					var vd = new Using.VariableDeclaration (enumerator_variable.LocalInfo, Location.Null);
					vd.Initializer = init;
					statement = new Using (vd, statement, Location.Null);
				}

				return statement.Resolve (ec);
			}

			protected override void DoEmit (EmitContext ec)
			{
				enumerator_variable.LocalInfo.CreateBuilder (ec);

				if (init != null)
					init.EmitStatement (ec);

				statement.Emit (ec);
			}

			#region IErrorHandler Members

			bool OverloadResolver.IErrorHandler.AmbiguousCandidates (ResolveContext ec, MemberSpec best, MemberSpec ambiguous)
			{
				ec.Report.SymbolRelatedToPreviousError (best);
				ec.Report.Warning (278, 2, expr.Location,
					"`{0}' contains ambiguous implementation of `{1}' pattern. Method `{2}' is ambiguous with method `{3}'",
					expr.Type.GetSignatureForError (), "enumerable",
					best.GetSignatureForError (), ambiguous.GetSignatureForError ());

				ambiguous_getenumerator_name = true;
				return true;
			}

			bool OverloadResolver.IErrorHandler.ArgumentMismatch (ResolveContext rc, MemberSpec best, Argument arg, int index)
			{
				return false;
			}

			bool OverloadResolver.IErrorHandler.NoArgumentMatch (ResolveContext rc, MemberSpec best)
			{
				return false;
			}

			bool OverloadResolver.IErrorHandler.TypeInferenceFailed (ResolveContext rc, MemberSpec best)
			{
				return false;
			}

			#endregion
		}

		Expression type;
		LocalVariable variable;
		Expression expr;
		Block body;

		public Foreach (Expression type, LocalVariable var, Expression expr, Statement stmt, Block body, Location l)
			: base (stmt)
		{
			this.type = type;
			this.variable = var;
			this.expr = expr;
			this.body = body;
			loc = l;
		}

		public Expression Expr {
			get { return expr; }
		}

		public Expression TypeExpression {
			get { return type; }
		}

		public LocalVariable Variable {
			get { return variable; }
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			base.MarkReachable (rc);

			body.MarkReachable (rc);

			return rc;
		}

		public override bool Resolve (BlockContext ec)
		{
			expr = expr.Resolve (ec);
			if (expr == null)
				return false;

			if (expr.IsNull) {
				ec.Report.Error (186, loc, "Use of null is not valid in this context");
				return false;
			}

			body.AddStatement (Statement);

			if (expr.Type.BuiltinType == BuiltinTypeSpec.Type.String) {
				Statement = new ArrayForeach (this, 1);
			} else if (expr.Type is ArrayContainer) {
				Statement = new ArrayForeach (this, ((ArrayContainer) expr.Type).Rank);
			} else {
				if (expr.eclass == ExprClass.MethodGroup || expr is AnonymousMethodExpression) {
					ec.Report.Error (446, expr.Location, "Foreach statement cannot operate on a `{0}'",
						expr.ExprClassName);
					return false;
				}

				Statement = new CollectionForeach (this, variable, expr);
			}

			return base.Resolve (ec);
		}

		protected override void DoEmit (EmitContext ec)
		{
			Label old_begin = ec.LoopBegin, old_end = ec.LoopEnd;
			ec.LoopBegin = ec.DefineLabel ();
			ec.LoopEnd = ec.DefineLabel ();

			if (!(Statement is Block))
				ec.BeginCompilerScope ();

			variable.CreateBuilder (ec);

			Statement.Emit (ec);

			if (!(Statement is Block))
				ec.EndScope ();

			ec.LoopBegin = old_begin;
			ec.LoopEnd = old_end;
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			expr.FlowAnalysis (fc);

			var da = fc.BranchDefiniteAssignment ();
			body.FlowAnalysis (fc);
			fc.DefiniteAssignment = da;
			return false;
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			Foreach target = (Foreach) t;

			target.type = type.Clone (clonectx);
			target.expr = expr.Clone (clonectx);
			target.body = (Block) body.Clone (clonectx);
			target.Statement = Statement.Clone (clonectx);
		}
		
		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}

	
}