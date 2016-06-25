using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ExclusiveOrExpression : Expression
    {
 			public ExclusiveOrExpression _exclusive_or_expression;
			public AndExpression _and_expression;

			[Rule("<exclusive or expression> ::= <exclusive or expression> '^' <and expression>")]
			public ExclusiveOrExpression(ExclusiveOrExpression _ExclusiveOrExpression, Semantic _symbol41,AndExpression _AndExpression)
				{
				_exclusive_or_expression = _ExclusiveOrExpression;
				_and_expression = _AndExpression;
				}
			[Rule("<exclusive or expression> ::= <and expression>")]
			public ExclusiveOrExpression(AndExpression _AndExpression)
				{
				_and_expression = _AndExpression;
				}
}
}
