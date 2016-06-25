using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class SyncStatement : Statement
    {
 			public Expression _expression;
			public Statement _statement;

			[Rule("<sync statement> ::= sync '(' <expression> ')' <statement>")]
			public SyncStatement( Semantic _symbol148, Semantic _symbol20,Expression _Expression, Semantic _symbol21,Statement _Statement)
				{
				_expression = _Expression;
				_statement = _Statement;
				}
}
}
