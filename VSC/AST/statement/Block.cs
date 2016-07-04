namespace VSC.AST {

	/// <summary>
	///   Block represents a C# block.
	/// </summary>
	///
	/// <remarks>
	///   This class is used in a number of places: either to represent
	///   explicit blocks that the programmer places or implicit blocks.
	///
	///   Implicit blocks are used as labels or to introduce variable
	///   declarations.
	///
	///   Top-level blocks derive from Block, and they are called ToplevelBlock
	///   they contain extra information that is not necessary on normal blocks.
	/// </remarks>
	public class Block : Statement {
		/*[Flags]
		public enum Flags
		{
			Unchecked = 1,
			ReachableEnd = 8,
			Unsafe = 16,
			HasCapturedVariable = 64,
			HasCapturedThis = 1 << 7,
			IsExpressionTree = 1 << 8,
			CompilerGenerated = 1 << 9,
			HasAsyncModifier = 1 << 10,
			Resolved = 1 << 11,
			YieldBlock = 1 << 12,
			AwaitBlock = 1 << 13,
			FinallyBlock = 1 << 14,
			CatchBlock = 1 << 15,
			Iterator = 1 << 20,
			NoFlowAnalysis = 1 << 21,
			InitializationEmitted = 1 << 22
		}

		public Block Parent;
		public Location StartLocation;
		public Location EndLocation;

		public ExplicitBlock Explicit;
		public ParametersBlock ParametersBlock;

		protected Flags flags;

		//
		// The statements in this block
		//
		protected List<Statement> statements;

		protected List<Statement> scope_initializers;

		int? resolving_init_idx;

		Block original;

#if DEBUG
		static int id;
		public int ID = id++;

		static int clone_id_counter;
		int clone_id;
#endif

//		int assignable_slots;

		public Block (Block parent, Location start, Location end)
			: this (parent, 0, start, end)
		{
		}

		public Block (Block parent, Flags flags, Location start, Location end)
		{
			if (parent != null) {
				// the appropriate constructors will fixup these fields
				ParametersBlock = parent.ParametersBlock;
				Explicit = parent.Explicit;
			}
			
			this.Parent = parent;
			this.flags = flags;
			this.StartLocation = start;
			this.EndLocation = end;
			this.loc = start;
			statements = new List<Statement> (4);

			this.original = this;
		}

		#region Properties

		public Block Original {
			get {
				return original;
			}
			protected set {
				original = value;
			}
		}

		public bool IsCompilerGenerated {
			get { return (flags & Flags.CompilerGenerated) != 0; }
			set { flags = value ? flags | Flags.CompilerGenerated : flags & ~Flags.CompilerGenerated; }
		}


		public bool IsCatchBlock {
			get {
				return (flags & Flags.CatchBlock) != 0;
			}
		}

		public bool IsFinallyBlock {
			get {
				return (flags & Flags.FinallyBlock) != 0;
			}
		}

		public bool Unchecked {
			get { return (flags & Flags.Unchecked) != 0; }
			set { flags = value ? flags | Flags.Unchecked : flags & ~Flags.Unchecked; }
		}

		public bool Unsafe {
			get { return (flags & Flags.Unsafe) != 0; }
			set { flags |= Flags.Unsafe; }
		}

		public List<Statement> Statements {
			get { return statements; }
		}

		#endregion

		public void SetEndLocation (Location loc)
		{
			EndLocation = loc;
		}

		public void AddLabel (LabeledStatement target)
		{
			ParametersBlock.TopBlock.AddLabel (target.Name, target);
		}

		public void AddLocalName (LocalVariable li)
		{
			AddLocalName (li.Name, li);
		}

		public void AddLocalName (string name, INamedBlockVariable li)
		{
			ParametersBlock.TopBlock.AddLocalName (name, li, false);
		}

		public virtual void Error_AlreadyDeclared (string name, INamedBlockVariable variable, string reason)
		{
			if (reason == null) {
				Error_AlreadyDeclared (name, variable);
				return;
			}

			ParametersBlock.TopBlock.Report.Error (136, variable.Location,
				"A local variable named `{0}' cannot be declared in this scope because it would give a different meaning " +
				"to `{0}', which is already used in a `{1}' scope to denote something else",
				name, reason);
		}

		public virtual void Error_AlreadyDeclared (string name, INamedBlockVariable variable)
		{
			var pi = variable as ParametersBlock.ParameterInfo;
			if (pi != null) {
				pi.Parameter.Error_DuplicateName (ParametersBlock.TopBlock.Report);
			} else {
				ParametersBlock.TopBlock.Report.Error (128, variable.Location,
					"A local variable named `{0}' is already defined in this scope", name);
			}
		}
					
		public virtual void Error_AlreadyDeclaredTypeParameter (string name, Location loc)
		{
			ParametersBlock.TopBlock.Report.Error (412, loc,
				"The type parameter name `{0}' is the same as local variable or parameter name",
				name);
		}

		//
		// It should be used by expressions which require to
		// register a statement during resolve process.
		//
		public void AddScopeStatement (Statement s)
		{
			if (scope_initializers == null)
				scope_initializers = new List<Statement> ();

			//
			// Simple recursive helper, when resolve scope initializer another
			// new scope initializer can be added, this ensures it's initialized
			// before existing one. For now this can happen with expression trees
			// in base ctor initializer only
			//
			if (resolving_init_idx.HasValue) {
				scope_initializers.Insert (resolving_init_idx.Value, s);
				++resolving_init_idx;
			} else {
				scope_initializers.Add (s);
			}
		}

		public void InsertStatement (int index, Statement s)
		{
			statements.Insert (index, s);
		}
		
		public void AddStatement (Statement s)
		{
			statements.Add (s);
		}

		public LabeledStatement LookupLabel (string name)
		{
			return ParametersBlock.GetLabel (name, this);
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			if (rc.IsUnreachable)
				return rc;

			MarkReachableScope (rc);

			foreach (var s in statements) {
				rc = s.MarkReachable (rc);
				if (rc.IsUnreachable) {
					if ((flags & Flags.ReachableEnd) != 0)
						return new Reachability ();

					return rc;
				}
			}

			flags |= Flags.ReachableEnd;

			return rc;
		}

		public void MarkReachableScope (Reachability rc)
		{
			base.MarkReachable (rc);

			if (scope_initializers != null) {
				foreach (var si in scope_initializers)
					si.MarkReachable (rc);
			}
		}

		public override bool Resolve (BlockContext bc)
		{
			if ((flags & Flags.Resolved) != 0)
				return true;

			Block prev_block = bc.CurrentBlock;
			bc.CurrentBlock = this;

			//
			// Compiler generated scope statements
			//
			if (scope_initializers != null) {
				for (resolving_init_idx = 0; resolving_init_idx < scope_initializers.Count; ++resolving_init_idx) {
					scope_initializers[resolving_init_idx.Value].Resolve (bc);
				}

				resolving_init_idx = null;
			}

			bool ok = true;
			int statement_count = statements.Count;
			for (int ix = 0; ix < statement_count; ix++){
				Statement s = statements [ix];

				if (!s.Resolve (bc)) {
					ok = false;
					statements [ix] = new EmptyStatement (s.loc);
					continue;
				}
			}

			bc.CurrentBlock = prev_block;

			flags |= Flags.Resolved;
			return ok;
		}

		protected override void DoEmit (EmitContext ec)
		{
			for (int ix = 0; ix < statements.Count; ix++){
				statements [ix].Emit (ec);
			}
		}

		public override void Emit (EmitContext ec)
		{
			if (scope_initializers != null)
				EmitScopeInitializers (ec);

			DoEmit (ec);
		}

		protected void EmitScopeInitializers (EmitContext ec)
		{
			foreach (Statement s in scope_initializers)
				s.Emit (ec);
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			if (scope_initializers != null) {
				foreach (var si in scope_initializers)
					si.FlowAnalysis (fc);
			}

			return DoFlowAnalysis (fc, 0);	
		}

		bool DoFlowAnalysis (FlowAnalysisContext fc, int startIndex)
		{
			bool end_unreachable = !reachable;
			bool goto_flow_analysis = startIndex != 0;
			for (; startIndex < statements.Count; ++startIndex) {
				var s = statements[startIndex];

				end_unreachable = s.FlowAnalysis (fc);
				if (s.IsUnreachable) {
					statements [startIndex] = RewriteUnreachableStatement (s);
					continue;
				}

				//
				// Statement end reachability is needed mostly due to goto support. Consider
				//
				// if (cond) {
				//    goto X;
				// } else {
				//    goto Y;
				// }
				// X:
				//
				// X label is reachable only via goto not as another statement after if. We need
				// this for flow-analysis only to carry variable info correctly.
				//
				if (end_unreachable) {
					bool after_goto_case = goto_flow_analysis && s is GotoCase;

					var f = s as TryFinally;
					if (f != null && !f.FinallyBlock.HasReachableClosingBrace) {
						//
						// Special case for try-finally with unreachable code after
						// finally block. Try block has to include leave opcode but there is
						// no label to leave to after unreachable finally block closing
						// brace. This sentinel ensures there is always IL instruction to
						// leave to even if we know it'll never be reached.
						//
						statements.Insert (startIndex + 1, new SentinelStatement ());
					} else {
						for (++startIndex; startIndex < statements.Count; ++startIndex) {
							s = statements [startIndex];
							if (s is SwitchLabel) {
								if (!after_goto_case)
									s.FlowAnalysis (fc);

								break;
							}

							if (s.IsUnreachable) {
								s.FlowAnalysis (fc);
								statements [startIndex] = RewriteUnreachableStatement (s);
							}
						}
					}

					//
					// Idea is to stop after goto case because goto case will always have at least same
					// variable assigned as switch case label. This saves a lot for complex goto case tests
					//
					if (after_goto_case)
						break;

					continue;
				}

				var lb = s as LabeledStatement;
				if (lb != null && fc.AddReachedLabel (lb))
					break;
			}

			//
			// The condition should be true unless there is forward jumping goto
			// 
			// if (this is ExplicitBlock && end_unreachable != Explicit.HasReachableClosingBrace)
			//	Debug.Fail ();

			return !Explicit.HasReachableClosingBrace;
		}

		static Statement RewriteUnreachableStatement (Statement s)
		{
			// LAMESPEC: It's not clear whether declararion statement should be part of reachability
			// analysis. Even csc report unreachable warning for it but it's actually used hence
			// we try to emulate this behaviour
			//
			// Consider:
			// 	goto L;
			//	int v;
			// L:
			//	v = 1;

			if (s is BlockVariable || s is EmptyStatement || s is SentinelStatement)
				return s;

			return new EmptyStatement (s.loc);
		}

		public void ScanGotoJump (Statement label)
		{
			int i;
			for (i = 0; i < statements.Count; ++i) {
				if (statements[i] == label)
					break;
			}

			var rc = new Reachability ();
			for (++i; i < statements.Count; ++i) {
				var s = statements[i];
				rc = s.MarkReachable (rc);
				if (rc.IsUnreachable)
					return;
			}

			flags |= Flags.ReachableEnd;
		}

		public void ScanGotoJump (Statement label, FlowAnalysisContext fc)
		{
			int i;
			for (i = 0; i < statements.Count; ++i) {
				if (statements[i] == label)
					break;
			}

			DoFlowAnalysis (fc, ++i);
		}

#if DEBUG
		public override string ToString ()
		{
			return String.Format ("{0}: ID={1} Clone={2} Location={3}", GetType (), ID, clone_id != 0, StartLocation);
		}
#endif

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			Block target = (Block) t;
#if DEBUG
			target.clone_id = ++clone_id_counter;
#endif

			clonectx.AddBlockMap (this, target);
			if (original != this)
				clonectx.AddBlockMap (original, target);

			target.ParametersBlock = (ParametersBlock) (ParametersBlock == this ? target : clonectx.RemapBlockCopy (ParametersBlock));
			target.Explicit = (ExplicitBlock) (Explicit == this ? target : clonectx.LookupBlock (Explicit));

			if (Parent != null)
				target.Parent = clonectx.RemapBlockCopy (Parent);

			target.statements = new List<Statement> (statements.Count);
			foreach (Statement s in statements)
				target.statements.Add (s.Clone (clonectx));
		}

		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}

	
}