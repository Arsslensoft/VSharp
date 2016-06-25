using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class PreIncrementExpression : Expression
    {
 			public Expression _unary_expression;

			[Rule("<pre increment expression> ::= '++' <unary expression>")]
			public PreIncrementExpression( Semantic _symbol52,Expression _UnaryExpression)
				{
				_unary_expression = _UnaryExpression;
				}
}
}
