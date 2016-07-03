namespace VSC.AST {

public class Catch : Statement
	{
		/*class CatchVariableStore : Statement
		{
			readonly Catch ctch;

			public CatchVariableStore (Catch ctch)
			{
				this.ctch = ctch;
			}

			protected override void CloneTo (CloneContext clonectx, Statement target)
			{
			}

			protected override void DoEmit (EmitContext ec)
			{
				// Emits catch variable debug information inside correct block
				ctch.EmitCatchVariableStore (ec);
			}

			protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
			{
				return true;
			}
		}

		class FilterStatement : Statement
		{
			readonly Catch ctch;

			public FilterStatement (Catch ctch)
			{
				this.ctch = ctch;
			}

			protected override void CloneTo (CloneContext clonectx, Statement target)
			{
			}

			protected override void DoEmit (EmitContext ec)
			{
				if (ctch.li != null) {
					if (ctch.hoisted_temp != null)
						ctch.hoisted_temp.Emit (ec);
					else
						ctch.li.Emit (ec);

					if (!ctch.IsGeneral && ctch.type.Kind == MemberKind.TypeParameter)
						ec.Emit (OpCodes.Box, ctch.type);
				}

				var expr_start = ec.DefineLabel ();
				var end = ec.DefineLabel ();

				ec.Emit (OpCodes.Brtrue_S, expr_start);
				ec.EmitInt (0);
				ec.Emit (OpCodes.Br, end);
				ec.MarkLabel (expr_start);

				ctch.Filter.Emit (ec);

				ec.MarkLabel (end);
				ec.Emit (OpCodes.Endfilter);
				ec.BeginFilterHandler ();
				ec.Emit (OpCodes.Pop);
			}

			protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
			{
				ctch.Filter.FlowAnalysis (fc);
				return true;
			}

			public override bool Resolve (BlockContext bc)
			{
				ctch.Filter = ctch.Filter.Resolve (bc);

				if (ctch.Filter != null) {
					if (ctch.Filter.ContainsEmitWithAwait ()) {
						bc.Report.Error (7094, ctch.Filter.Location, "The `await' operator cannot be used in the filter expression of a catch clause");
					}

					var c = ctch.Filter as Constant;
					if (c != null && !c.IsDefaultValue) {
						bc.Report.Warning (7095, 1, ctch.Filter.Location, "Exception filter expression is a constant");
					}
				}

				return true;
			}
		}

		ExplicitBlock block;
		LocalVariable li;
		FullNamedExpression type_expr;
		CompilerAssign assign;
		TypeSpec type;
		LocalTemporary hoisted_temp;

		public Catch (ExplicitBlock block, Location loc)
		{
			this.block = block;
			this.loc = loc;
		}

		#region Properties

		public ExplicitBlock Block {
			get {
				return block;
			}
		}

		public TypeSpec CatchType {
			get {
				return type;
			}
		}

		public Expression Filter {
			get; set;
		}

		public bool IsGeneral {
			get {
				return type_expr == null;
			}
		}

		public FullNamedExpression TypeExpression {
			get {
				return type_expr;
			}
			set {
				type_expr = value;
			}
		}

		public LocalVariable Variable {
			get {
				return li;
			}
			set {
				li = value;
			}
		}

		#endregion

		protected override void DoEmit (EmitContext ec)
		{
			if (Filter != null) {
				ec.BeginExceptionFilterBlock ();
				ec.Emit (OpCodes.Isinst, IsGeneral ? ec.BuiltinTypes.Object : CatchType);

				if (Block.HasAwait) {
					Block.EmitScopeInitialization (ec);
				} else {
					Block.Emit (ec);
				}

				return;
			}

			if (IsGeneral)
				ec.BeginCatchBlock (ec.BuiltinTypes.Object);
			else
				ec.BeginCatchBlock (CatchType);

			if (li == null)
				ec.Emit (OpCodes.Pop);

			if (Block.HasAwait) {
				if (li != null)
					EmitCatchVariableStore (ec);
			} else {
				Block.Emit (ec);
			}
		}

		void EmitCatchVariableStore (EmitContext ec)
		{
			li.CreateBuilder (ec);

			//
			// For hoisted catch variable we have to use a temporary local variable
			// for captured variable initialization during storey setup because variable
			// needs to be on the stack after storey instance for stfld operation
			//
			if (li.HoistedVariant != null) {
				hoisted_temp = new LocalTemporary (li.Type);
				hoisted_temp.Store (ec);

				// switch to assignment from temporary variable and not from top of the stack
				assign.UpdateSource (hoisted_temp);
			}
		}

		public override bool Resolve (BlockContext bc)
		{
			using (bc.Set (ResolveContext.Options.CatchScope)) {
				if (type_expr == null) {
					if (CreateExceptionVariable (bc.Module.Compiler.BuiltinTypes.Object)) {
						if (!block.HasAwait || Filter != null)
							block.AddScopeStatement (new CatchVariableStore (this));

						Expression source = new EmptyExpression (li.Type);
						assign = new CompilerAssign (new LocalVariableReference (li, Location.Null), source, Location.Null);
						Block.AddScopeStatement (new StatementExpression (assign, Location.Null));
					}
				} else {
					type = type_expr.ResolveAsType (bc);
					if (type == null)
						return false;

					if (li == null)
						CreateExceptionVariable (type);

					if (type.BuiltinType != BuiltinTypeSpec.Type.Exception && !TypeSpec.IsBaseClass (type, bc.BuiltinTypes.Exception, false)) {
						bc.Report.Error (155, loc, "The type caught or thrown must be derived from System.Exception");
					} else if (li != null) {
						li.Type = type;
						li.PrepareAssignmentAnalysis (bc);

						// source variable is at the top of the stack
						Expression source = new EmptyExpression (li.Type);
						if (li.Type.IsGenericParameter)
							source = new UnboxCast (source, li.Type);

						if (!block.HasAwait || Filter != null)
							block.AddScopeStatement (new CatchVariableStore (this));

						//
						// Uses Location.Null to hide from symbol file
						//
						assign = new CompilerAssign (new LocalVariableReference (li, Location.Null), source, Location.Null);
						Block.AddScopeStatement (new StatementExpression (assign, Location.Null));
					}
				}

				if (Filter != null) {
					Block.AddScopeStatement (new FilterStatement (this));
				}

				Block.SetCatchBlock ();
				return Block.Resolve (bc);
			}
		}

		bool CreateExceptionVariable (TypeSpec type)
		{
			if (!Block.HasAwait)
				return false;

			// TODO: Scan the block for rethrow expression
			//if (!Block.HasRethrow)
			//	return;

			li = LocalVariable.CreateCompilerGenerated (type, block, Location.Null);
			return true;
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			if (li != null && !li.IsCompilerGenerated) {
				fc.SetVariableAssigned (li.VariableInfo, true);
			}

			return block.FlowAnalysis (fc);
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			base.MarkReachable (rc);

			var c = Filter as Constant;
			if (c != null && c.IsDefaultValue)
				return Reachability.CreateUnreachable ();

			return block.MarkReachable (rc);
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			Catch target = (Catch) t;

			if (type_expr != null)
				target.type_expr = (FullNamedExpression) type_expr.Clone (clonectx);

			if (Filter != null)
				target.Filter = Filter.Clone (clonectx);

			target.block = (ExplicitBlock) clonectx.LookupBlock (block);
		}*/
	}

	
}