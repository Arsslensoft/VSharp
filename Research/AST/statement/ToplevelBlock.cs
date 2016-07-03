namespace VSC.AST {

//
	//
	//
	public class ToplevelBlock : ParametersBlock
	{
	/*	LocalVariable this_variable;
		CompilerContext compiler;
		Dictionary<string, object> names;

		List<ExplicitBlock> this_references;

		public ToplevelBlock (CompilerContext ctx, Location loc)
			: this (ctx, ParametersCompiled.EmptyReadOnlyParameters, loc)
		{
		}

		public ToplevelBlock (CompilerContext ctx, ParametersCompiled parameters, Location start, Flags flags = 0)
			: base (parameters, start)
		{
			this.compiler = ctx;
			this.flags = flags;
			top_block = this;

			ProcessParameters ();
		}

		//
		// Recreates a top level block from parameters block. Used for
		// compiler generated methods where the original block comes from
		// explicit child block. This works for already resolved blocks
		// only to ensure we resolve them in the correct flow order
		//
		public ToplevelBlock (ParametersBlock source, ParametersCompiled parameters)
			: base (source, parameters)
		{
			this.compiler = source.TopBlock.compiler;
			top_block = this;
		}

		public bool IsIterator {
			get {
				return (flags & Flags.Iterator) != 0;
			}
			set {
				flags = value ? flags | Flags.Iterator : flags & ~Flags.Iterator;
			}
		}

		public Report Report {
			get {
				return compiler.Report;
			}
		}

		//
		// Used by anonymous blocks to track references of `this' variable
		//
		public List<ExplicitBlock> ThisReferencesFromChildrenBlock {
			get {
				return this_references;
			}
		}

		//
		// Returns the "self" instance variable of this block.
		// See AddThisVariable() for more information.
		//
		public LocalVariable ThisVariable {
			get {
				return this_variable;
			}
		}

		public void AddLocalName (string name, INamedBlockVariable li, bool ignoreChildrenBlocks)
		{
			if (names == null)
				names = new Dictionary<string, object> ();

			object value;
			if (!names.TryGetValue (name, out value)) {
				names.Add (name, li);
				return;
			}

			INamedBlockVariable existing = value as INamedBlockVariable;
			List<INamedBlockVariable> existing_list;
			if (existing != null) {
				existing_list = new List<INamedBlockVariable> ();
				existing_list.Add (existing);
				names[name] = existing_list;
			} else {
				existing_list = (List<INamedBlockVariable>) value;
			}

			//
			// A collision checking between local names
			//
			var variable_block = li.Block.Explicit;
			for (int i = 0; i < existing_list.Count; ++i) {
				existing = existing_list[i];
				Block b = existing.Block.Explicit;

				// Collision at same level
				if (variable_block == b) {
					li.Block.Error_AlreadyDeclared (name, li);
					break;
				}

				// Collision with parent
				Block parent = variable_block;
				while ((parent = parent.Parent) != null) {
					if (parent == b) {
						li.Block.Error_AlreadyDeclared (name, li, "parent or current");
						i = existing_list.Count;
						break;
					}
				}

				if (!ignoreChildrenBlocks && variable_block.Parent != b.Parent) {
					// Collision with children
					while ((b = b.Parent) != null) {
						if (variable_block == b) {
							li.Block.Error_AlreadyDeclared (name, li, "child");
							i = existing_list.Count;
							break;
						}
					}
				}
			}

			existing_list.Add (li);
		}

		public void AddLabel (string name, LabeledStatement label)
		{
			if (labels == null)
				labels = new Dictionary<string, object> ();

			object value;
			if (!labels.TryGetValue (name, out value)) {
				labels.Add (name, label);
				return;
			}

			LabeledStatement existing = value as LabeledStatement;
			List<LabeledStatement> existing_list;
			if (existing != null) {
				existing_list = new List<LabeledStatement> ();
				existing_list.Add (existing);
				labels[name] = existing_list;
			} else {
				existing_list = (List<LabeledStatement>) value;
			}

			//
			// A collision checking between labels
			//
			for (int i = 0; i < existing_list.Count; ++i) {
				existing = existing_list[i];
				Block b = existing.Block;

				// Collision at same level
				if (label.Block == b) {
					Report.SymbolRelatedToPreviousError (existing.loc, name);
					Report.Error (140, label.loc, "The label `{0}' is a duplicate", name);
					break;
				}

				// Collision with parent
				b = label.Block;
				while ((b = b.Parent) != null) {
					if (existing.Block == b) {
						Report.Error (158, label.loc,
							"The label `{0}' shadows another label by the same name in a contained scope", name);
						i = existing_list.Count;
						break;
					}
				}

				// Collision with with children
				b = existing.Block;
				while ((b = b.Parent) != null) {
					if (label.Block == b) {
						Report.Error (158, label.loc,
							"The label `{0}' shadows another label by the same name in a contained scope", name);
						i = existing_list.Count;
						break;
					}
				}
			}

			existing_list.Add (label);
		}

		public void AddThisReferenceFromChildrenBlock (ExplicitBlock block)
		{
			if (this_references == null)
				this_references = new List<ExplicitBlock> ();

			if (!this_references.Contains (block))
				this_references.Add (block);
		}

		public void RemoveThisReferenceFromChildrenBlock (ExplicitBlock block)
		{
			this_references.Remove (block);
		}

		//
		// Creates an arguments set from all parameters, useful for method proxy calls
		//
		public Arguments GetAllParametersArguments ()
		{
			int count = parameters.Count;
			Arguments args = new Arguments (count);
			for (int i = 0; i < count; ++i) {
				var pi = parameter_info[i];
				var arg_expr = GetParameterReference (i, pi.Location);

				Argument.AType atype_modifier;
				switch (pi.Parameter.ParameterModifier & Parameter.Modifier.RefOutMask) {
				case Parameter.Modifier.REF:
					atype_modifier = Argument.AType.Ref;
					break;
				case Parameter.Modifier.OUT:
					atype_modifier = Argument.AType.Out;
					break;
				default:
					atype_modifier = 0;
					break;
				}

				args.Add (new Argument (arg_expr, atype_modifier));
			}

			return args;
		}

		//
		// Lookup inside a block, the returned value can represent 3 states
		//
		// true+variable: A local name was found and it's valid
		// false+variable: A local name was found in a child block only
		// false+null: No local name was found
		//
		public bool GetLocalName (string name, Block block, ref INamedBlockVariable variable)
		{
			if (names == null)
				return false;

			object value;
			if (!names.TryGetValue (name, out value))
				return false;

			variable = value as INamedBlockVariable;
			Block b = block;
			if (variable != null) {
				do {
					if (variable.Block == b.Original)
						return true;

					b = b.Parent;
				} while (b != null);

				b = variable.Block;
				do {
					if (block == b)
						return false;

					b = b.Parent;
				} while (b != null);
			} else {
				List<INamedBlockVariable> list = (List<INamedBlockVariable>) value;
				for (int i = 0; i < list.Count; ++i) {
					variable = list[i];
					do {
						if (variable.Block == b.Original)
							return true;

						b = b.Parent;
					} while (b != null);

					b = variable.Block;
					do {
						if (block == b)
							return false;

						b = b.Parent;
					} while (b != null);

					b = block;
				}
			}

			variable = null;
			return false;
		}

		public void IncludeBlock (ParametersBlock pb, ToplevelBlock block)
		{
			if (block.names != null) {
				foreach (var n in block.names) {
					var variable = n.Value as INamedBlockVariable;
					if (variable != null) {
						if (variable.Block.ParametersBlock == pb)
							AddLocalName (n.Key, variable, false);
						continue;
					}

					foreach (var v in (List<INamedBlockVariable>) n.Value)
						if (v.Block.ParametersBlock == pb)
							AddLocalName (n.Key, v, false);
				}
			}
		}

		// <summary>
		//   This is used by non-static `struct' constructors which do not have an
		//   initializer - in this case, the constructor must initialize all of the
		//   struct's fields.  To do this, we add a "self" variable and use the flow
		//   analysis code to ensure that it's been fully initialized before control
		//   leaves the constructor.
		// </summary>
		public void AddThisVariable (BlockContext bc)
		{
			if (this_variable != null)
				throw new InternalErrorException (StartLocation.ToString ());

			this_variable = new LocalVariable (this, "self", LocalVariable.Flags.IsThis | LocalVariable.Flags.Used, StartLocation);
			this_variable.Type = bc.CurrentType;
			this_variable.PrepareAssignmentAnalysis (bc);
		}

		public override void CheckControlExit (FlowAnalysisContext fc, DefiniteAssignmentBitSet dat)
		{
			//
			// If we're a non-static struct constructor which doesn't have an
			// initializer, then we must initialize all of the struct's fields.
			//
			if (this_variable != null)
				this_variable.IsThisAssigned (fc, this);

			base.CheckControlExit (fc, dat);
		}

		public HashSet<LocalVariable> GetUndeclaredVariables ()
		{
			if (names == null)
				return null;

			HashSet<LocalVariable> variables = null;

			foreach (var entry in names) {
				var complex = entry.Value as List<INamedBlockVariable>;
				if (complex != null) {
					foreach (var centry in complex) {
						if (IsUndeclaredVariable (centry)) {
							if (variables == null)
								variables = new HashSet<LocalVariable> ();

							variables.Add ((LocalVariable) centry);
						}
					}
				} else if (IsUndeclaredVariable ((INamedBlockVariable)entry.Value)) {
					if (variables == null)
						variables = new HashSet<LocalVariable> ();

					variables.Add ((LocalVariable)entry.Value);					
				}
			}

			return variables;
		}

		static bool IsUndeclaredVariable (INamedBlockVariable namedBlockVariable)
		{
			var lv = namedBlockVariable as LocalVariable;
			return lv != null && !lv.IsDeclared;
		}

		public void SetUndeclaredVariables (HashSet<LocalVariable> undeclaredVariables)
		{
			if (names == null)
				return;
			
			foreach (var entry in names) {
				var complex = entry.Value as List<INamedBlockVariable>;
				if (complex != null) {
					foreach (var centry in complex) {
						var lv = centry as LocalVariable;
						if (lv != null && undeclaredVariables.Contains (lv)) {
							lv.Type = null;
						}
					}
				} else {
					var lv = entry.Value as LocalVariable;
					if (lv != null && undeclaredVariables.Contains (lv))
						lv.Type = null;
				}
			}
		}

		public override void Emit (EmitContext ec)
		{
			if (Report.Errors > 0)
				return;

			try {
			if (IsCompilerGenerated) {
				using (ec.With (BuilderContext.Options.OmitDebugInfo, true)) {
					base.Emit (ec);
				}
			} else {
				base.Emit (ec);
			}

			//
			// If `HasReturnLabel' is set, then we already emitted a
			// jump to the end of the method, so we must emit a `ret'
			// there.
			//
			// Unfortunately, System.Reflection.Emit automatically emits
			// a leave to the end of a finally block.  This is a problem
			// if no code is following the try/finally block since we may
			// jump to a point after the end of the method.
			// As a workaround, we're always creating a return label in
			// this case.
			//
			if (ec.HasReturnLabel || HasReachableClosingBrace) {
				if (ec.HasReturnLabel)
					ec.MarkLabel (ec.ReturnLabel);

				if (ec.EmitAccurateDebugInfo && !IsCompilerGenerated)
					ec.Mark (EndLocation);

				if (ec.ReturnType.Kind != MemberKind.Void)
					ec.Emit (OpCodes.Ldloc, ec.TemporaryReturn ());

				ec.Emit (OpCodes.Ret);
			}

			} catch (Exception e) {
				throw new InternalErrorException (e, StartLocation);
			}
		}

		public bool Resolve (BlockContext bc, IMethodData md)
		{
			if (resolved)
				return true;

			var errors = bc.Report.Errors;

			base.Resolve (bc);

			if (bc.Report.Errors > errors)
				return false;

			MarkReachable (new Reachability ());

			if (HasReachableClosingBrace && bc.ReturnType.Kind != MemberKind.Void) {
				// TODO: var md = bc.CurrentMemberDefinition;
				bc.Report.Error (161, md.Location, "`{0}': not all code paths return a value", md.GetSignatureForError ());
			}

			if ((flags & Flags.NoFlowAnalysis) != 0)
				return true;

			var fc = new FlowAnalysisContext (bc.Module.Compiler, this, bc.AssignmentInfoOffset);
			try {
				FlowAnalysis (fc);
			} catch (Exception e) {
				throw new InternalErrorException (e, StartLocation);
			}

			return true;
		}*/
	}
	
}