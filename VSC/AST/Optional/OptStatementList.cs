using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptStatementList : Semantic {
 			public StatementList _statement_list;

			[Rule("<opt statement list> ::= ")]
			public OptStatementList()
				{
				}
			[Rule("<opt statement list> ::= <statement list>")]
			public OptStatementList(StatementList _StatementList)
				{
				_statement_list = _StatementList;
				}
}
}
