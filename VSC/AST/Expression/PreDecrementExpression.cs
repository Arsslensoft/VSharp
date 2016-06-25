using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class PreDecrementExpression : Expression
    {
 			public Expression _unary_expression;

			[Rule("<pre decrement expression> ::= '--' <unary expression>")]
			public PreDecrementExpression( Semantic _symbol9,Expression _UnaryExpression)
				{
				_unary_expression = _UnaryExpression;
				}
}
}
