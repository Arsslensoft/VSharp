using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class WhileThenStatement : Statement
    {
 			public Expression _expression;
			public Statement _then_statement;

			[Rule("<while then statement> ::= while '(' <expression> ')' <Then Statement>")]
            public WhileThenStatement(Semantic _symbol164, Semantic _symbol20, Expression _Expression, Semantic _symbol21, Statement _Statement)
				{
				_expression = _Expression;
				_then_statement = _Statement;
				}
}
}
