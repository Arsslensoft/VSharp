namespace VSC.AST {

public class BlockVariable : Statement
	{
		/*Expression initializer;
		protected FullNamedExpression type_expr;
		protected LocalVariable li;
		protected List<BlockVariableDeclarator> declarators;
		TypeSpec type;

		public BlockVariable (FullNamedExpression type, LocalVariable li)
		{
			this.type_expr = type;
			this.li = li;
			this.loc = type_expr.Location;
		}

		protected BlockVariable (LocalVariable li)
		{
			this.li = li;
		}

		#region Properties

		public List<BlockVariableDeclarator> Declarators {
			get {
				return declarators;
			}
		}

		public Expression Initializer {
			get {
				return initializer;
			}
			set {
				initializer = value;
			}
		}

		public FullNamedExpression TypeExpression {
			get {
				return type_expr;
			}
		}

		public LocalVariable Variable {
			get {
				return li;
			}
		}

		#endregion

		public void AddDeclarator (BlockVariableDeclarator decl)
		{
			if (declarators == null)
				declarators = new List<BlockVariableDeclarator> ();

			declarators.Add (decl);
		}

		static void CreateEvaluatorVariable (BlockContext bc, LocalVariable li)
		{
			if (bc.Report.Errors != 0)
				return;

			var container = bc.CurrentMemberDefinition.Parent.PartialContainer;

			Field f = new Field (container, new TypeExpression (li.Type, li.Location), Modifiers.PUBLIC | Modifiers.STATIC,
				new MemberName (li.Name, li.Location), null);

			container.AddField (f);
			f.Define ();

			li.HoistedVariant = new HoistedEvaluatorVariable (f);
			li.SetIsUsed ();
		}

		public override bool Resolve (BlockContext bc)
		{
			return Resolve (bc, true);
		}

		public bool Resolve (BlockContext bc, bool resolveDeclaratorInitializers)
		{
			if (type == null && !li.IsCompilerGenerated) {
				var vexpr = type_expr as VarExpr;

				//
				// C# 3.0 introduced contextual keywords (var) which behaves like a type if type with
				// same name exists or as a keyword when no type was found
				//
				if (vexpr != null && !vexpr.IsPossibleType (bc)) {
					if (bc.Module.Compiler.Settings.Version < LanguageVersion.V_3)
						bc.Report.FeatureIsNotAvailable (bc.Module.Compiler, loc, "implicitly typed local variable");

					if (li.IsFixed) {
						bc.Report.Error (821, loc, "A fixed statement cannot use an implicitly typed local variable");
						return false;
					}

					if (li.IsConstant) {
						bc.Report.Error (822, loc, "An implicitly typed local variable cannot be a constant");
						return false;
					}

					if (Initializer == null) {
						bc.Report.Error (818, loc, "An implicitly typed local variable declarator must include an initializer");
						return false;
					}

					if (declarators != null) {
						bc.Report.Error (819, loc, "An implicitly typed local variable declaration cannot include multiple declarators");
						declarators = null;
					}

					Initializer = Initializer.Resolve (bc);
					if (Initializer != null) {
						((VarExpr) type_expr).InferType (bc, Initializer);
						type = type_expr.Type;
					} else {
						// Set error type to indicate the var was placed correctly but could
						// not be infered
						//
						// var a = missing ();
						//
						type = InternalType.ErrorType;
					}
				}

				if (type == null) {
					type = type_expr.ResolveAsType (bc);
					if (type == null)
						return false;

					if (li.IsConstant && !type.IsConstantCompatible) {
						Const.Error_InvalidConstantType (type, loc, bc.Report);
					}
				}

				if (type.IsStatic)
					FieldBase.Error_VariableOfStaticClass (loc, li.Name, type, bc.Report);

				li.Type = type;
			}

			bool eval_global = bc.Module.Compiler.Settings.StatementMode && bc.CurrentBlock is ToplevelBlock;
			if (eval_global) {
				CreateEvaluatorVariable (bc, li);
			} else if (type != InternalType.ErrorType) {
				li.PrepareAssignmentAnalysis (bc);
			}

			if (initializer != null) {
				initializer = ResolveInitializer (bc, li, initializer);
				// li.Variable.DefinitelyAssigned 
			}

			if (declarators != null) {
				foreach (var d in declarators) {
					d.Variable.Type = li.Type;
					if (eval_global) {
						CreateEvaluatorVariable (bc, d.Variable);
					} else if (type != InternalType.ErrorType) {
						d.Variable.PrepareAssignmentAnalysis (bc);
					}

					if (d.Initializer != null && resolveDeclaratorInitializers) {
						d.Initializer = ResolveInitializer (bc, d.Variable, d.Initializer);
						// d.Variable.DefinitelyAssigned 
					} 
				}
			}

			return true;
		}

		protected virtual Expression ResolveInitializer (BlockContext bc, LocalVariable li, Expression initializer)
		{
			var a = new SimpleAssign (li.CreateReferenceExpression (bc, li.Location), initializer, li.Location);
			return a.ResolveStatement (bc);
		}

		protected override void DoEmit (EmitContext ec)
		{
			li.CreateBuilder (ec);

			if (Initializer != null && !IsUnreachable)
				((ExpressionStatement) Initializer).EmitStatement (ec);

			if (declarators != null) {
				foreach (var d in declarators) {
					d.Variable.CreateBuilder (ec);
					if (d.Initializer != null && !IsUnreachable) {
						ec.Mark (d.Variable.Location);
						((ExpressionStatement) d.Initializer).EmitStatement (ec);
					}
				}
			}
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			if (Initializer != null)
				Initializer.FlowAnalysis (fc);

			if (declarators != null) {
				foreach (var d in declarators) {
					if (d.Initializer != null)
						d.Initializer.FlowAnalysis (fc);
				}
			}

			return false;
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			var init = initializer as ExpressionStatement;
			if (init != null)
				init.MarkReachable (rc);

			return base.MarkReachable (rc);
		}

		protected override void CloneTo (CloneContext clonectx, Statement target)
		{
			BlockVariable t = (BlockVariable) target;

			if (type_expr != null)
				t.type_expr = (FullNamedExpression) type_expr.Clone (clonectx);

			if (initializer != null)
				t.initializer = initializer.Clone (clonectx);

			if (declarators != null) {
				t.declarators = null;
				foreach (var d in declarators)
					t.AddDeclarator (d.Clone (clonectx));
			}
		}

		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}


}