using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class PostIncrementExpression : Expression
    {
 			public Expression _primary_expression;

			[Rule("<post increment expression> ::= <primary expression> '++'")]
			public PostIncrementExpression(Expression _PrimaryExpression, Semantic _symbol52)
				{
				_primary_expression = _PrimaryExpression;
				}
}
}
