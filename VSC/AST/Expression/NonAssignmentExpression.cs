using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class NonAssignmentExpression : Expression
    {
 			public ConditionalExpression _conditional_expression;
			public LambdaExpression _lambda_expression;

			[Rule("<non assignment expression> ::= <conditional expression>")]
			public NonAssignmentExpression(ConditionalExpression _ConditionalExpression)
				{
				_conditional_expression = _ConditionalExpression;
				}
			[Rule("<non assignment expression> ::= <lambda expression>")]
			public NonAssignmentExpression(LambdaExpression _LambdaExpression)
				{
				_lambda_expression = _LambdaExpression;
				}
}
}
