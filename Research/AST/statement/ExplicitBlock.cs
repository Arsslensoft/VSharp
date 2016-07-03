namespace VSC.AST {
public class ExplicitBlock : Block
	{
		/*protected AnonymousMethodStorey am_storey;

		public ExplicitBlock (Block parent, Location start, Location end)
			: this (parent, (Flags) 0, start, end)
		{
		}

		public ExplicitBlock (Block parent, Flags flags, Location start, Location end)
			: base (parent, flags, start, end)
		{
			this.Explicit = this;
		}

		#region Properties

		public AnonymousMethodStorey AnonymousMethodStorey {
			get {
				return am_storey;
			}
		}

		public bool HasAwait {
			get {
				return (flags & Flags.AwaitBlock) != 0;
			}
		}

		public bool HasCapturedThis {
			set {
				flags = value ? flags | Flags.HasCapturedThis : flags & ~Flags.HasCapturedThis;
			}
			get {
				return (flags & Flags.HasCapturedThis) != 0;
			}
		}

		//
		// Used to indicate that the block has reference to parent
		// block and cannot be made static when defining anonymous method
		//
		public bool HasCapturedVariable {
			set {
				flags = value ? flags | Flags.HasCapturedVariable : flags & ~Flags.HasCapturedVariable;
			}
			get {
				return (flags & Flags.HasCapturedVariable) != 0;
			}
		}

		public bool HasReachableClosingBrace {
		    get {
		        return (flags & Flags.ReachableEnd) != 0;
		    }
			set {
				flags = value ? flags | Flags.ReachableEnd : flags & ~Flags.ReachableEnd;
			}
		}

		public bool HasYield {
			get {
				return (flags & Flags.YieldBlock) != 0;
			}
		}

		#endregion

		//
		// Creates anonymous method storey in current block
		//
		public AnonymousMethodStorey CreateAnonymousMethodStorey (ResolveContext ec)
		{
			//
			// Return same story for iterator and async blocks unless we are
			// in nested anonymous method
			//
			if (ec.CurrentAnonymousMethod is StateMachineInitializer && ParametersBlock.Original == ec.CurrentAnonymousMethod.Block.Original)
				return ec.CurrentAnonymousMethod.Storey;

			if (am_storey == null) {
				MemberBase mc = ec.MemberContext as MemberBase;

				//
				// Creates anonymous method storey for this block
				//
				am_storey = new AnonymousMethodStorey (this, ec.CurrentMemberDefinition.Parent.PartialContainer, mc, ec.CurrentTypeParameters, "AnonStorey", MemberKind.Class);
			}

			return am_storey;
		}

		public void EmitScopeInitialization (EmitContext ec)
		{
			if ((flags & Flags.InitializationEmitted) != 0)
				return;

			if (am_storey != null) {
				DefineStoreyContainer (ec, am_storey);
				am_storey.EmitStoreyInstantiation (ec, this);
			}

			if (scope_initializers != null)
				EmitScopeInitializers (ec);

			flags |= Flags.InitializationEmitted;
		}

		public override void Emit (EmitContext ec)
		{
			if (Parent != null)
				ec.BeginScope ();

			EmitScopeInitialization (ec);

			if (ec.EmitAccurateDebugInfo && !IsCompilerGenerated && ec.Mark (StartLocation)) {
				ec.Emit (OpCodes.Nop);
			}

			DoEmit (ec);

			if (Parent != null)
				ec.EndScope ();

			if (ec.EmitAccurateDebugInfo && HasReachableClosingBrace && !(this is ParametersBlock) &&
				!IsCompilerGenerated && ec.Mark (EndLocation)) {
				ec.Emit (OpCodes.Nop);
			}
		}

		protected void DefineStoreyContainer (EmitContext ec, AnonymousMethodStorey storey)
		{
			if (ec.CurrentAnonymousMethod != null && ec.CurrentAnonymousMethod.Storey != null) {
				storey.SetNestedStoryParent (ec.CurrentAnonymousMethod.Storey);
				storey.Mutator = ec.CurrentAnonymousMethod.Storey.Mutator;
			}

			//
			// Creates anonymous method storey
			//
			storey.CreateContainer ();
			storey.DefineContainer ();

			if (Original.Explicit.HasCapturedThis && Original.ParametersBlock.TopBlock.ThisReferencesFromChildrenBlock != null) {

				//
				// Only first storey in path will hold this reference. All children blocks will
				// reference it indirectly using $ref field
				//
				for (Block b = Original.Explicit; b != null; b = b.Parent) {
					if (b.Parent != null) {
						var s = b.Parent.Explicit.AnonymousMethodStorey;
						if (s != null) {
							storey.HoistedThis = s.HoistedThis;
							break;
						}
					}

					if (b.Explicit == b.Explicit.ParametersBlock && b.Explicit.ParametersBlock.StateMachine != null) {
						if (storey.HoistedThis == null)
							storey.HoistedThis = b.Explicit.ParametersBlock.StateMachine.HoistedThis;

						if (storey.HoistedThis != null)
							break;
					}
				}
				
				//
				// We are the first storey on path and 'this' has to be hoisted
				//
				if (storey.HoistedThis == null || !(storey.Parent is HoistedStoreyClass)) {
					foreach (ExplicitBlock ref_block in Original.ParametersBlock.TopBlock.ThisReferencesFromChildrenBlock) {
						//
						// ThisReferencesFromChildrenBlock holds all reference even if they
						// are not on this path. It saves some memory otherwise it'd have to
						// be in every explicit block. We run this check to see if the reference
						// is valid for this storey
						//
						Block block_on_path = ref_block;
						for (; block_on_path != null && block_on_path != Original; block_on_path = block_on_path.Parent);

						if (block_on_path == null)
							continue;

						if (storey.HoistedThis == null) {
							storey.AddCapturedThisField (ec, null);
						}

						for (ExplicitBlock b = ref_block; b.AnonymousMethodStorey != storey; b = b.Parent.Explicit) {
							ParametersBlock pb;
							AnonymousMethodStorey b_storey = b.AnonymousMethodStorey;

							if (b_storey != null) {
								//
								// Don't add storey cross reference for `this' when the storey ends up not
								// beeing attached to any parent
								//
								if (b.ParametersBlock.StateMachine == null) {
									AnonymousMethodStorey s = null;
									for (Block ab = b.AnonymousMethodStorey.OriginalSourceBlock.Parent; ab != null; ab = ab.Parent) {
										s = ab.Explicit.AnonymousMethodStorey;
										if (s != null)
											break;
									}

									// Needs to be in sync with AnonymousMethodBody::DoCreateMethodHost
									if (s == null) {
										var parent = storey == null || storey.Kind == MemberKind.Struct ? null : storey;
										b.AnonymousMethodStorey.AddCapturedThisField (ec, parent);
										break;
									}

								}

								//
								// Stop propagation inside same top block
								//
								if (b.ParametersBlock == ParametersBlock.Original) {
									b_storey.AddParentStoreyReference (ec, storey);
//									b_storey.HoistedThis = storey.HoistedThis;
									break;
								}

								b = pb = b.ParametersBlock;
							} else {
								pb = b as ParametersBlock;
							}

							if (pb != null && pb.StateMachine != null) {
								if (pb.StateMachine == storey)
									break;

								//
								// If we are state machine with no parent. We can hook into parent without additional
 								// reference and capture this directly
								//
								ExplicitBlock parent_storey_block = pb;
								while (parent_storey_block.Parent != null) {
									parent_storey_block = parent_storey_block.Parent.Explicit;
									if (parent_storey_block.AnonymousMethodStorey != null) {
										break;
									}
								}

								if (parent_storey_block.AnonymousMethodStorey == null) {
									if (pb.StateMachine.HoistedThis == null) {
										pb.StateMachine.AddCapturedThisField (ec, null);
										b.HasCapturedThis = true;
									}

									continue;
								}

								var parent_this_block = pb;
								while (parent_this_block.Parent != null) {
									parent_this_block = parent_this_block.Parent.ParametersBlock;
									if (parent_this_block.StateMachine != null && parent_this_block.StateMachine.HoistedThis != null) {
										break;
									}
								}

								//
								// Add reference to closest storey which holds captured this
								//
								pb.StateMachine.AddParentStoreyReference (ec, parent_this_block.StateMachine ?? storey);
							}

							//
							// Add parent storey reference only when this is not captured directly
							//
							if (b_storey != null) {
								b_storey.AddParentStoreyReference (ec, storey);
								b_storey.HoistedThis = storey.HoistedThis;
							}
						}
					}
				}
			}

			var ref_blocks = storey.ReferencesFromChildrenBlock;
			if (ref_blocks != null) {
				foreach (ExplicitBlock ref_block in ref_blocks) {
					for (ExplicitBlock b = ref_block; b.AnonymousMethodStorey != storey; b = b.Parent.Explicit) {
						if (b.AnonymousMethodStorey != null) {
							b.AnonymousMethodStorey.AddParentStoreyReference (ec, storey);

							//
							// Stop propagation inside same top block
							//
							if (b.ParametersBlock == ParametersBlock.Original)
								break;

							b = b.ParametersBlock;
						}

						var pb = b as ParametersBlock;
						if (pb != null && pb.StateMachine != null) {
							if (pb.StateMachine == storey)
								break;

							pb.StateMachine.AddParentStoreyReference (ec, storey);
						}

						b.HasCapturedVariable = true;
					}
				}
			}

			storey.Define ();
			storey.PrepareEmit ();
			storey.Parent.PartialContainer.AddCompilerGeneratedClass (storey);
		}

		public void RegisterAsyncAwait ()
		{
			var block = this;
			while ((block.flags & Flags.AwaitBlock) == 0) {
				block.flags |= Flags.AwaitBlock;

				if (block is ParametersBlock)
					return;

				block = block.Parent.Explicit;
			}
		}

		public void RegisterIteratorYield ()
		{
			ParametersBlock.TopBlock.IsIterator = true;

			var block = this;
			while ((block.flags & Flags.YieldBlock) == 0) {
				block.flags |= Flags.YieldBlock;

				if (block.Parent == null)
					return;

				block = block.Parent.Explicit;
			}
		}

		public void SetCatchBlock ()
		{
			flags |= Flags.CatchBlock;
		}

		public void SetFinallyBlock ()
		{
			flags |= Flags.FinallyBlock;
		}

		public void WrapIntoDestructor (TryFinally tf, ExplicitBlock tryBlock)
		{
			tryBlock.statements = statements;
			statements = new List<Statement> (1);
			statements.Add (tf);
		}*/
	}


}