using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class SyncThenStatement : Statement
    {
 			public Expression _expression;
			public Statement _then_statement;

			[Rule("<sync then statement> ::= sync '(' <expression> ')' <Then Statement>")]
            public SyncThenStatement(Semantic _symbol148, Semantic _symbol20, Expression _Expression, Semantic _symbol21, Statement _Statement)
				{
				_expression = _Expression;
				_then_statement = _Statement;
				}
}
}
