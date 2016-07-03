namespace VSC.AST {

public abstract class LoopStatement : Statement
	{
/*		protected LoopStatement (Statement statement)
		{
			Statement = statement;
		}
        protected LoopStatement(Statement statement, Statement elsestmt)
        {
            Statement = statement;
            ElseStatement = elsestmt;
        }
		public Statement Statement { get; set; }
        public Statement ElseStatement { get; set; }
        public Location ElseLocation { get; set; }
		public override bool Resolve (BlockContext bc)
		{
			var prev_loop = bc.EnclosingLoop;
			var prev_los = bc.EnclosingLoopOrSwitch;
			bc.EnclosingLoopOrSwitch = bc.EnclosingLoop = this;
			var ok = Statement.Resolve (bc);
            if(ElseStatement != null)
                ok &= ElseStatement.Resolve(bc);
			bc.EnclosingLoopOrSwitch = prev_los;
			bc.EnclosingLoop = prev_loop;

			return ok;
		}

        public void EmitElse(EmitContext ec, Expression expr, Label lb, Location el)
        {
         
            ec.Mark(el);
            expr.EmitBranchable(ec, lb, true);
            ElseStatement.Emit(ec);
            ec.Emit(OpCodes.Br, ec.LoopEnd);
        }
		//
		// Needed by possibly infinite loops statements (for, while) and switch statment
		//
		public virtual void AddEndDefiniteAssignment (FlowAnalysisContext fc)
		{
		}

		public virtual void SetEndReachable ()
		{
		}

		public virtual void SetIteratorReachable ()
		{
		}*/
	}
	
}