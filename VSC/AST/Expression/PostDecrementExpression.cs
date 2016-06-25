using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class PostDecrementExpression : Expression
    {
 			public Expression _primary_expression;

			[Rule("<post decrement expression> ::= <primary expression> '--'")]
			public PostDecrementExpression(Expression _PrimaryExpression, Semantic _symbol9)
				{
				_primary_expression = _PrimaryExpression;
				}
}
}
