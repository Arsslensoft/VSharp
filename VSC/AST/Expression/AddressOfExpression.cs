using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class AddressOfExpression : Expression
    {
 			public Expression _unary_expression;

			[Rule("<address of expression> ::= '&' <unary expression>")]
			public AddressOfExpression( Semantic _symbol17,Expression _UnaryExpression)
				{
				_unary_expression = _UnaryExpression;
				}
}
}
