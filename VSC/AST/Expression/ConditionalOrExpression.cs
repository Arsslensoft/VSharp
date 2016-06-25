using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ConditionalOrExpression : Expression
    {
 			public ConditionalOrExpression _conditional_or_expression;
			public ConditionalAndExpression _conditional_and_expression;

			[Rule("<conditional or expression> ::= <conditional or expression> '||' <conditional and expression>")]
			public ConditionalOrExpression(ConditionalOrExpression _ConditionalOrExpression, Semantic _symbol45,ConditionalAndExpression _ConditionalAndExpression)
				{
				_conditional_or_expression = _ConditionalOrExpression;
				_conditional_and_expression = _ConditionalAndExpression;
				}
			[Rule("<conditional or expression> ::= <conditional and expression>")]
			public ConditionalOrExpression(ConditionalAndExpression _ConditionalAndExpression)
				{
				_conditional_and_expression = _ConditionalAndExpression;
				}
}
}
