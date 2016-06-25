using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ConditionalAndExpression : Expression
    {
 			public ConditionalAndExpression _conditional_and_expression;
			public InclusiveOrExpression _inclusive_or_expression;

			[Rule("<conditional and expression> ::= <conditional and expression> '&&' <inclusive or expression>")]
			public ConditionalAndExpression(ConditionalAndExpression _ConditionalAndExpression, Semantic _symbol18,InclusiveOrExpression _InclusiveOrExpression)
				{
				_conditional_and_expression = _ConditionalAndExpression;
				_inclusive_or_expression = _InclusiveOrExpression;
				}
			[Rule("<conditional and expression> ::= <inclusive or expression>")]
			public ConditionalAndExpression(InclusiveOrExpression _InclusiveOrExpression)
				{
				_inclusive_or_expression = _InclusiveOrExpression;
				}
}
}
