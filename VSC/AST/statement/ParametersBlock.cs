namespace VSC.AST {
//
	// ParametersBlock was introduced to support anonymous methods
	// and lambda expressions
	// 
	public class ParametersBlock : ExplicitBlock
	{
		/*public class ParameterInfo : INamedBlockVariable
		{
			readonly ParametersBlock block;
			readonly int index;
			public VariableInfo VariableInfo;
			bool is_locked;

			public ParameterInfo (ParametersBlock block, int index)
			{
				this.block = block;
				this.index = index;
			}

			#region Properties

			public ParametersBlock Block {
				get {
					return block;
				}
			}

			Block INamedBlockVariable.Block {
				get {
					return block;
				}
			}

			public bool IsDeclared {
				get {
					return true;
				}
			}

			public bool IsParameter {
				get {
					return true;
				}
			}

			public bool IsLocked {
				get {
					return is_locked;
				}
				set {
					is_locked = value;
				}
			}

			public Location Location {
				get {
					return Parameter.Location;
				}
			}

			public Parameter Parameter {
				get {
					return block.Parameters [index];
				}
			}

			public TypeSpec ParameterType {
				get {
					return Parameter.Type;
				}
			}

			#endregion

			public Expression CreateReferenceExpression (ResolveContext rc, Location loc)
			{
				return new ParameterReference (this, loc);
			}
		}

		// 
		// Block is converted into an expression
		//
		sealed class BlockScopeExpression : Expression
		{
			Expression child;
			readonly ParametersBlock block;

			public BlockScopeExpression (Expression child, ParametersBlock block)
			{
				this.child = child;
				this.block = block;
			}

			public override bool ContainsEmitWithAwait ()
			{
				return child.ContainsEmitWithAwait ();
			}

			public override Expression CreateExpressionTree (ResolveContext ec)
			{
				throw new NotSupportedException ();
			}

			protected override Expression DoResolve (ResolveContext ec)
			{
				if (child == null)
					return null;

				child = child.Resolve (ec);
				if (child == null)
					return null;

				eclass = child.eclass;
				type = child.Type;
				return this;
			}

			public override void Emit (EmitContext ec)
			{
				block.EmitScopeInitializers (ec);
				child.Emit (ec);
			}
		}

		protected ParametersCompiled parameters;
		protected ParameterInfo[] parameter_info;
		protected bool resolved;
		protected ToplevelBlock top_block;
		protected StateMachine state_machine;
		protected Dictionary<string, object> labels;

		public ParametersBlock (Block parent, ParametersCompiled parameters, Location start, Flags flags = 0)
			: base (parent, 0, start, start)
		{
			if (parameters == null)
				throw new ArgumentNullException ("parameters");

			this.parameters = parameters;
			ParametersBlock = this;

			this.flags |= flags | (parent.ParametersBlock.flags & (Flags.YieldBlock | Flags.AwaitBlock));

			this.top_block = parent.ParametersBlock.top_block;
			ProcessParameters ();
		}

		protected ParametersBlock (ParametersCompiled parameters, Location start)
			: base (null, 0, start, start)
		{
			if (parameters == null)
				throw new ArgumentNullException ("parameters");

			this.parameters = parameters;
			ParametersBlock = this;
		}

		//
		// It's supposed to be used by method body implementation of anonymous methods
		//
		protected ParametersBlock (ParametersBlock source, ParametersCompiled parameters)
			: base (null, 0, source.StartLocation, source.EndLocation)
		{
			this.parameters = parameters;
			this.statements = source.statements;
			this.scope_initializers = source.scope_initializers;

			this.resolved = true;
			this.reachable = source.reachable;
			this.am_storey = source.am_storey;
			this.state_machine = source.state_machine;
			this.flags = source.flags & Flags.ReachableEnd;

			ParametersBlock = this;

			//
			// Overwrite original for comparison purposes when linking cross references
			// between anonymous methods
			//
			Original = source.Original;
		}

		#region Properties

		public bool IsAsync {
			get {
				return (flags & Flags.HasAsyncModifier) != 0;
			}
			set {
				flags = value ? flags | Flags.HasAsyncModifier : flags & ~Flags.HasAsyncModifier;
			}
		}

		//
		// Block has been converted to expression tree
		//
		public bool IsExpressionTree {
			get {
				return (flags & Flags.IsExpressionTree) != 0;
			}
		}

		//
		// The parameters for the block.
		//
		public ParametersCompiled Parameters {
			get {
				return parameters;
			}
            set
            {
                parameters = value;
            }
		}

		public StateMachine StateMachine {
			get {
				return state_machine;
			}
		}

		public ToplevelBlock TopBlock {
			get {
				return top_block;
			}
			set {
				top_block = value;
			}
		}

		public bool Resolved {
			get {
				return (flags & Flags.Resolved) != 0;
			}
		}

		public int TemporaryLocalsCount { get; set; }

		#endregion

		//
		// Checks whether all `out' parameters have been assigned.
		//
		public void CheckControlExit (FlowAnalysisContext fc)
		{
			CheckControlExit (fc, fc.DefiniteAssignment);
		}

		public virtual void CheckControlExit (FlowAnalysisContext fc, DefiniteAssignmentBitSet dat)
		{
			if (parameter_info == null)
				return;

			foreach (var p in parameter_info) {
				if (p.VariableInfo == null)
					continue;

				if (p.VariableInfo.IsAssigned (dat))
					continue;

				fc.Report.Error (177, p.Location,
					"The out parameter `{0}' must be assigned to before control leaves the current method",
					p.Parameter.Name);
			}					
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			base.CloneTo (clonectx, t);

			var target = (ParametersBlock) t;

			//
			// Clone label statements as well as they contain block reference
			//
			var pb = this;
			while (true) {
				if (pb.labels != null) {
					target.labels = new Dictionary<string, object> ();

					foreach (var entry in pb.labels) {
						var list = entry.Value as List<LabeledStatement>;

						if (list != null) {
							var list_clone = new List<LabeledStatement> ();
							foreach (var lentry in list) {
								list_clone.Add (RemapLabeledStatement (lentry, clonectx.RemapBlockCopy (lentry.Block)));
							}

							target.labels.Add (entry.Key, list_clone);
						} else {
							var labeled = (LabeledStatement) entry.Value;
							target.labels.Add (entry.Key, RemapLabeledStatement (labeled, clonectx.RemapBlockCopy (labeled.Block)));
						}
					}

					break;
				}

				if (pb.Parent == null)
					break;

				pb = pb.Parent.ParametersBlock;
			}
		}

		public override Expression CreateExpressionTree (ResolveContext ec)
		{
			if (statements.Count == 1) {
				Expression expr = statements[0].CreateExpressionTree (ec);
				if (scope_initializers != null)
					expr = new BlockScopeExpression (expr, this);

				return expr;
			}

			return base.CreateExpressionTree (ec);
		}

		public override void Emit (EmitContext ec)
		{
			if (state_machine != null && state_machine.OriginalSourceBlock != this) {
				DefineStoreyContainer (ec, state_machine);
				state_machine.EmitStoreyInstantiation (ec, this);
			}

			base.Emit (ec);
		}

		public void EmitEmbedded (EmitContext ec)
		{
			if (state_machine != null && state_machine.OriginalSourceBlock != this) {
				DefineStoreyContainer (ec, state_machine);
				state_machine.EmitStoreyInstantiation (ec, this);
			}

			base.Emit (ec);
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			var res = base.DoFlowAnalysis (fc);

			if (HasReachableClosingBrace)
				CheckControlExit (fc);

			return res;
		}

		public LabeledStatement GetLabel (string name, Block block)
		{
			//
			// Cloned parameters blocks can have their own cloned version of top-level labels
			//
			if (labels == null) {
				if (Parent != null)
					return Parent.ParametersBlock.GetLabel (name, block);

				return null;
			}

			object value;
			if (!labels.TryGetValue (name, out value)) {
				return null;
			}

			var label = value as LabeledStatement;
			Block b = block;
			if (label != null) {
				if (IsLabelVisible (label, b))
					return label;

			} else {
				List<LabeledStatement> list = (List<LabeledStatement>) value;
				for (int i = 0; i < list.Count; ++i) {
					label = list[i];
					if (IsLabelVisible (label, b))
						return label;
				}
			}

			return null;
		}

		static bool IsLabelVisible (LabeledStatement label, Block b)
		{
			do {
				if (label.Block == b)
					return true;
				b = b.Parent;
			} while (b != null);

			return false;
		}

		public ParameterInfo GetParameterInfo (Parameter p)
		{
			for (int i = 0; i < parameters.Count; ++i) {
				if (parameters[i] == p)
					return parameter_info[i];
			}

			throw new ArgumentException ("Invalid parameter");
		}

		public ParameterReference GetParameterReference (int index, Location loc)
		{
			return new ParameterReference (parameter_info[index], loc);
		}

		public Statement PerformClone (ref HashSet<LocalVariable> undeclaredVariables)
		{
			undeclaredVariables = TopBlock.GetUndeclaredVariables ();

			CloneContext clonectx = new CloneContext ();
			return Clone (clonectx);
		}

		protected void ProcessParameters ()
		{
			if (parameters.Count == 0)
				return;

			parameter_info = new ParameterInfo[parameters.Count];
			for (int i = 0; i < parameter_info.Length; ++i) {
				var p = parameters.FixedParameters[i];
				if (p == null)
					continue;

				// TODO: Should use Parameter only and more block there
				parameter_info[i] = new ParameterInfo (this, i);
				if (p.Name != null)
					AddLocalName (p.Name, parameter_info[i]);
			}
		}

		LabeledStatement RemapLabeledStatement (LabeledStatement stmt, Block dst)
		{
			var src = stmt.Block;

			//
			// Cannot remap label block if the label was not yet cloned which
			// can happen in case of anonymous method inside anoynymous method
			// with a label. But in this case we don't care because goto cannot
			// jump of out anonymous method
			//
			if (src.ParametersBlock != this)
				return stmt;

			var src_stmts = src.Statements;
			for (int i = 0; i < src_stmts.Count; ++i) {
				if (src_stmts[i] == stmt)
					return (LabeledStatement) dst.Statements[i];
			}

			throw new InternalErrorException ("Should never be reached");
		}

		public override bool Resolve (BlockContext bc)
		{
			// TODO: if ((flags & Flags.Resolved) != 0)

			if (resolved)
				return true;

			resolved = true;

			if (bc.HasSet (ResolveContext.Options.ExpressionTreeConversion))
				flags |= Flags.IsExpressionTree;

			try {
				PrepareAssignmentAnalysis (bc);

				if (!base.Resolve (bc))
					return false;

			} catch (Exception e) {
				if (e is CompletionResult || bc.Report.IsDisabled || e is FatalException || bc.Report.Printer is NullReportPrinter || bc.Module.Compiler.Settings.BreakOnInternalError)
					throw;

				if (bc.CurrentBlock != null) {
					bc.Report.Error (584, bc.CurrentBlock.StartLocation, "Internal compiler error: {0}", e.Message);
				} else {
					bc.Report.Error (587, "Internal compiler error: {0}", e.Message);
				}
			}

			//
			// If an asynchronous body of F is either an expression classified as nothing, or a 
			// statement block where no return statements have expressions, the inferred return type is Task
			//
			if (IsAsync) {
				var am = bc.CurrentAnonymousMethod as AnonymousMethodBody;
				if (am != null && am.ReturnTypeInference != null && !am.ReturnTypeInference.HasBounds (0)) {
					am.ReturnTypeInference = null;
					am.ReturnType = bc.Module.PredefinedTypes.Task.TypeSpec;
					return true;
				}
			}

			return true;
		}

		void PrepareAssignmentAnalysis (BlockContext bc)
		{
			for (int i = 0; i < parameters.Count; ++i) {
				var par = parameters.FixedParameters[i];

				if ((par.ModFlags & Parameter.Modifier.OUT) == 0)
					continue;

				parameter_info [i].VariableInfo = VariableInfo.Create (bc, (Parameter) par);
			}
		}

		public ToplevelBlock ConvertToIterator (IMethodData method, TypeDefinition host, TypeSpec iterator_type, bool is_enumerable)
		{
			var iterator = new Iterator (this, method, host, iterator_type, is_enumerable);
			var stateMachine = new IteratorStorey (iterator);

			state_machine = stateMachine;
			iterator.SetStateMachine (stateMachine);

			var tlb = new ToplevelBlock (host.Compiler, Parameters, Location.Null, Flags.CompilerGenerated);
			tlb.Original = this;
			tlb.state_machine = stateMachine;
			tlb.AddStatement (new Return (iterator, iterator.Location));
			return tlb;
		}

		public ParametersBlock ConvertToAsyncTask (IMemberContext context, TypeDefinition host, ParametersCompiled parameters, TypeSpec returnType, TypeSpec delegateType, Location loc)
		{
			for (int i = 0; i < parameters.Count; i++) {
				Parameter p = parameters[i];
				Parameter.Modifier mod = p.ModFlags;
				if ((mod & Parameter.Modifier.RefOutMask) != 0) {
					host.Compiler.Report.Error (1988, p.Location,
						"Async methods cannot have ref or out parameters");
					return this;
				}

				if (p is ArglistParameter) {
					host.Compiler.Report.Error (4006, p.Location,
						"__arglist is not allowed in parameter list of async methods");
					return this;
				}

				if (parameters.Types[i].IsPointer) {
					host.Compiler.Report.Error (4005, p.Location,
						"Async methods cannot have unsafe parameters");
					return this;
				}
			}

			if (!HasAwait) {
				host.Compiler.Report.Warning (1998, 1, loc,
					"Async block lacks `await' operator and will run synchronously");
			}

			var block_type = host.Module.Compiler.BuiltinTypes.Void;
			var initializer = new AsyncInitializer (this, host, block_type);
			initializer.Type = block_type;
			initializer.DelegateType = delegateType;

			var stateMachine = new AsyncTaskStorey (this, context, initializer, returnType);

			state_machine = stateMachine;
			initializer.SetStateMachine (stateMachine);

			const Flags flags = Flags.CompilerGenerated;

			var b = this is ToplevelBlock ?
				new ToplevelBlock (host.Compiler, Parameters, Location.Null, flags) :
				new ParametersBlock (Parent, parameters, Location.Null, flags | Flags.HasAsyncModifier);

			b.Original = this;
			b.state_machine = stateMachine;
			b.AddStatement (new AsyncInitializerStatement (initializer));
			return b;
		}*/
	}


}