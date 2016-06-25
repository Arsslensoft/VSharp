using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class WhileStatement : Statement
    {
 			public Expression _expression;
			public Statement _statement;

			[Rule("<while statement> ::= while '(' <expression> ')' <statement>")]
			public WhileStatement( Semantic _symbol164, Semantic _symbol20,Expression _Expression, Semantic _symbol21,Statement _Statement)
				{
				_expression = _Expression;
				_statement = _Statement;
				}
}
}
