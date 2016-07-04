using System.Collections.Generic;
namespace VSC.AST {
//
	// Simple version of statement list not requiring a block
	//
	public class StatementList : Statement
	{
		List<Statement> statements;

		public StatementList (Statement first, Statement second)
		{
			statements = new List<Statement> { first, second };
		}

		#region Properties
		public IList<Statement> Statements {
			get {
				return statements;
			}
		}
		#endregion

		public void Add(Statement statement)
		{
            statements.Add(statement);
		}

	
	}


}