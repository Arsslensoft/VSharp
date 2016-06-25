using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class PositiveExpression : Expression
    {
 			public Expression _unary_expression;

			[Rule("<positive expression> ::= '+' <unary expression>")]
			public PositiveExpression( Semantic _symbol51,Expression _UnaryExpression)
				{
				_unary_expression = _UnaryExpression;
				}
}
}
