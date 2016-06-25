using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ParenthesizedExpression : Expression
    {
 			public Expression _expression;

			[Rule("<parenthesized expression> ::= '(' <expression> ')'")]
			public ParenthesizedExpression( Semantic _symbol20,Expression _Expression, Semantic _symbol21)
				{
				_expression = _Expression;
				}
}
}
