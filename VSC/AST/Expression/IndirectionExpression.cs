using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class IndirectionExpression : Expression
    {
 			public Expression _unary_expression;

			[Rule("<indirection expression> ::= '*' <unary expression>")]
			public IndirectionExpression( Semantic _symbol22,Expression _UnaryExpression)
				{
				_unary_expression = _UnaryExpression;
				}
}
}
