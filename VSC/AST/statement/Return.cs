namespace VSC.AST {
/// <summary>
	///   Implements the return statement
	/// </summary>
	public class Return : ExitStatement
	{
		/*Expression expr;

		public Return (Expression expr, Location l)
		{
			this.expr = expr;
			loc = l;
		}

		#region Properties

		public Expression Expr {
			get {
				return expr;
			}
			protected set {
				expr = value;
			}
		}

		protected override bool IsLocalExit {
			get {
				return false;
			}
		}

		#endregion

		protected override bool DoResolve (BlockContext ec)
		{
			var block_return_type = ec.ReturnType;

			if (expr == null) {
				if (block_return_type.Kind == MemberKind.Void || block_return_type == InternalType.ErrorType)
					return true;

				//
				// Return must not be followed by an expression when
				// the method return type is Task
				//
				if (ec.CurrentAnonymousMethod is AsyncInitializer) {
					var storey = (AsyncTaskStorey) ec.CurrentAnonymousMethod.Storey;
					if (storey.ReturnType == ec.Module.PredefinedTypes.Task.TypeSpec) {
						//
						// Extra trick not to emit ret/leave inside awaiter body
						//
						expr = EmptyExpression.Null;
						return true;
					}

					if (storey.ReturnType.IsGenericTask)
						block_return_type = storey.ReturnType.TypeArguments[0];
				}

				if (ec.CurrentIterator != null) {
					Error_ReturnFromIterator (ec);
				} else if (block_return_type != InternalType.ErrorType) {
					ec.Report.Error (126, loc,
						"An object of a type convertible to `{0}' is required for the return statement",
						block_return_type.GetSignatureForError ());
				}

				return false;
			}

			expr = expr.Resolve (ec);

			AnonymousExpression am = ec.CurrentAnonymousMethod;
			if (am == null) {
				if (block_return_type.Kind == MemberKind.Void) {
					ec.Report.Error (127, loc,
						"`{0}': A return keyword must not be followed by any expression when method returns void",
						ec.GetSignatureForError ());

					return false;
				}
			} else {
				if (am.IsIterator) {
					Error_ReturnFromIterator (ec);
					return false;
				}

				var async_block = am as AsyncInitializer;
				if (async_block != null) {
					if (expr != null) {
						var storey = (AsyncTaskStorey) am.Storey;
						var async_type = storey.ReturnType;

						if (async_type == null && async_block.ReturnTypeInference != null) {
							if (expr.Type.Kind == MemberKind.Void && !(this is ContextualReturn))
								ec.Report.Error (4029, loc, "Cannot return an expression of type `void'");
							else
								async_block.ReturnTypeInference.AddCommonTypeBoundAsync (expr.Type);
							return true;
						}

						if (async_type.Kind == MemberKind.Void) {
							ec.Report.Error (8030, loc,
								"Anonymous function or lambda expression converted to a void returning delegate cannot return a value");
							return false;
						}

						if (!async_type.IsGenericTask) {
							if (this is ContextualReturn)
								return true;

							if (async_block.DelegateType != null) {
								ec.Report.Error (8031, loc,
									"Async lambda expression or anonymous method converted to a `Task' cannot return a value. Consider returning `Task<T>'");
							} else {
								ec.Report.Error (1997, loc,
									"`{0}': A return keyword must not be followed by an expression when async method returns `Task'. Consider using `Task<T>' return type",
									ec.GetSignatureForError ());
							}
							return false;
						}

						//
						// The return type is actually Task<T> type argument
						//
						if (expr.Type == async_type && async_type.TypeArguments [0] != ec.Module.PredefinedTypes.Task.TypeSpec) {
							ec.Report.Error (4016, loc,
								"`{0}': The return expression type of async method must be `{1}' rather than `Task<{1}>'",
								ec.GetSignatureForError (), async_type.TypeArguments[0].GetSignatureForError ());
						} else {
							block_return_type = async_type.TypeArguments[0];
						}
					}
				} else {
					if (block_return_type.Kind == MemberKind.Void) {
						ec.Report.Error (8030, loc,
							"Anonymous function or lambda expression converted to a void returning delegate cannot return a value");
						return false;
					}

					var l = am as AnonymousMethodBody;
					if (l != null && expr != null) {
						if (l.ReturnTypeInference != null) {
							l.ReturnTypeInference.AddCommonTypeBound (expr.Type);
							return true;
						}

						//
						// Try to optimize simple lambda. Only when optimizations are enabled not to cause
						// unexpected debugging experience
						//
						if (this is ContextualReturn && !ec.IsInProbingMode && ec.Module.Compiler.Settings.Optimize) {
							l.DirectMethodGroupConversion = expr.CanReduceLambda (l);
						}
					}
				}
			}

			if (expr == null)
				return false;

			if (expr.Type != block_return_type && expr.Type != InternalType.ErrorType) {
				expr = Convert.ImplicitConversionRequired (ec, expr, block_return_type, loc);

				if (expr == null) {
					if (am != null && block_return_type == ec.ReturnType) {
						ec.Report.Error (1662, loc,
							"Cannot convert `{0}' to delegate type `{1}' because some of the return types in the block are not implicitly convertible to the delegate return type",
							am.ContainerType, am.GetSignatureForError ());
					}
					return false;
				}
			}

			return true;			
		}
		
		protected override void DoEmit (EmitContext ec)
		{
			if (expr != null) {

				var async_body = ec.CurrentAnonymousMethod as AsyncInitializer;
				if (async_body != null) {
					var storey = (AsyncTaskStorey)async_body.Storey;
					Label exit_label = async_body.BodyEnd;

					//
					// It's null for await without async
					//
					if (storey.HoistedReturnValue != null) {
						//
						// Special case hoisted return value (happens in try/finally scenario)
						//
						if (ec.TryFinallyUnwind != null) {
							if (storey.HoistedReturnValue is VariableReference) {
								storey.HoistedReturnValue = ec.GetTemporaryField (storey.HoistedReturnValue.Type);
							}

							exit_label = TryFinally.EmitRedirectedReturn (ec, async_body);
						}

						var async_return = (IAssignMethod)storey.HoistedReturnValue;
						async_return.EmitAssign (ec, expr, false, false);
						ec.EmitEpilogue ();
					} else {
						expr.Emit (ec);

						if (ec.TryFinallyUnwind != null)
							exit_label = TryFinally.EmitRedirectedReturn (ec, async_body);
					}

					ec.Emit (OpCodes.Leave, exit_label);
					return;
				}

				expr.Emit (ec);
				ec.EmitEpilogue ();

				if (unwind_protect || ec.EmitAccurateDebugInfo)
					ec.Emit (OpCodes.Stloc, ec.TemporaryReturn ());
			}

			if (unwind_protect) {
				ec.Emit (OpCodes.Leave, ec.CreateReturnLabel ());
			} else if (ec.EmitAccurateDebugInfo) {
				ec.Emit (OpCodes.Br, ec.CreateReturnLabel ());
			} else {
				ec.Emit (OpCodes.Ret);
			}
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			if (expr != null)
				expr.FlowAnalysis (fc);

			base.DoFlowAnalysis (fc);
			return true;
		}

		void Error_ReturnFromIterator (ResolveContext rc)
		{
			rc.Report.Error (1622, loc,
				"Cannot return a value from iterators. Use the yield return statement to return a value, or yield break to end the iteration");
		}

		public override Reachability MarkReachable (Reachability rc)
		{
			base.MarkReachable (rc);
			return Reachability.CreateUnreachable ();
		}

		protected override void CloneTo (CloneContext clonectx, Statement t)
		{
			Return target = (Return) t;
			// It's null for simple return;
			if (expr != null)
				target.expr = expr.Clone (clonectx);
		}

		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}



}