using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class InclusiveOrExpression : Expression
    {
 			public InclusiveOrExpression _inclusive_or_expression;
			public ExclusiveOrExpression _exclusive_or_expression;

			[Rule("<inclusive or expression> ::= <inclusive or expression> '|' <exclusive or expression>")]
			public InclusiveOrExpression(InclusiveOrExpression _InclusiveOrExpression, Semantic _symbol44,ExclusiveOrExpression _ExclusiveOrExpression)
				{
				_inclusive_or_expression = _InclusiveOrExpression;
				_exclusive_or_expression = _ExclusiveOrExpression;
				}
			[Rule("<inclusive or expression> ::= <exclusive or expression>")]
			public InclusiveOrExpression(ExclusiveOrExpression _ExclusiveOrExpression)
				{
				_exclusive_or_expression = _ExclusiveOrExpression;
				}
}
}
