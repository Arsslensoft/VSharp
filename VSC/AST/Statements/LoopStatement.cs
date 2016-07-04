namespace VSC.AST {

public abstract class LoopStatement : Statement
	{
		protected LoopStatement (Statement statement)
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
		
	}
	
}