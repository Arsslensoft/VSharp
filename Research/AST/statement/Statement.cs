namespace VSC.AST {
	public abstract class Statement {
/*		public Location loc;
		protected bool reachable;

		public bool IsUnreachable {
			get {
				return !reachable;
			}
		}
		
		/// <summary>
		///   Resolves the statement, true means that all sub-statements
		///   did resolve ok.
		///  </summary>
		public virtual bool Resolve (BlockContext bc)
		{
      

			return true;
		}

		/// <summary>
		///   Return value indicates whether all code paths emitted return.
		/// </summary>
		protected abstract void DoEmit (EmitContext ec);

		public virtual void Emit (EmitContext ec)
		{
			ec.Mark (loc);
			DoEmit (ec);

			if (ec.StatementEpilogue != null) {
				ec.EmitEpilogue ();
			}
		}

		//
		// This routine must be overrided in derived classes and make copies
		// of all the data that might be modified if resolved
		// 
		protected abstract void CloneTo (CloneContext clonectx, Statement target);

		public Statement Clone (CloneContext clonectx)
		{
			Statement s = (Statement) this.MemberwiseClone ();
			CloneTo (clonectx, s);
			return s;
		}

		public virtual Expression CreateExpressionTree (ResolveContext ec)
		{
			ec.Report.Error (834, loc, "A lambda expression with statement body cannot be converted to an expresion tree");
			return null;
		}
		
		public virtual object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}

		//
		// Return value indicates whether statement has unreachable end
		//
		protected abstract bool DoFlowAnalysis (FlowAnalysisContext fc);

		public bool FlowAnalysis (FlowAnalysisContext fc)
		{
			if (reachable) {
				fc.UnreachableReported = false;
				var res = DoFlowAnalysis (fc);
				return res;
			}

			//
			// Special handling cases
			//
			if (this is Block) {
				return DoFlowAnalysis (fc);
			}

			if (this is EmptyStatement || loc.IsNull)
				return true;

			if (fc.UnreachableReported)
				return true;

			fc.Report.Warning (162, 2, loc, "Unreachable code detected");
			fc.UnreachableReported = true;
			return true;
		}

		public virtual Reachability MarkReachable (Reachability rc)
		{
			if (!rc.IsUnreachable)
				reachable = true;

			return rc;
		}

		protected void CheckExitBoundaries (BlockContext bc, Block scope)
		{
			if (bc.CurrentBlock.ParametersBlock.Original != scope.ParametersBlock.Original) {
				bc.Report.Error (1632, loc, "Control cannot leave the body of an anonymous method");
				return;
			}

			for (var b = bc.CurrentBlock; b != null && b != scope; b = b.Parent) {
				if (b.IsFinallyBlock) {
					Error_FinallyClauseExit (bc);
					break;
				}
			}
		}

		protected void Error_FinallyClauseExit (BlockContext bc)
		{
			bc.Report.Error (157, loc, "Control cannot leave the body of a finally clause");
		}
*/	}



}