using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class StatementList : Statement
    {
 			public Statement _statement;
			public StatementList _statement_list;

			[Rule("<statement list> ::= <statement>")]
			public StatementList(Statement _Statement)
				{
				_statement = _Statement;
				}
			[Rule("<statement list> ::= <statement list> <statement>")]
			public StatementList(StatementList _StatementList,Statement _Statement)
				{
				_statement_list = _StatementList;
				_statement = _Statement;
				}
}
}
