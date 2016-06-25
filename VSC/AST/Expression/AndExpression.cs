using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class AndExpression : Expression
    {
 			public AndExpression _and_expression;
			public EqualityExpression _equality_expression;

			[Rule("<and expression> ::= <and expression> '&' <equality expression>")]
			public AndExpression(AndExpression _AndExpression, Semantic _symbol17,EqualityExpression _EqualityExpression)
				{
				_and_expression = _AndExpression;
				_equality_expression = _EqualityExpression;
				}
			[Rule("<and expression> ::= <equality expression>")]
			public AndExpression(EqualityExpression _EqualityExpression)
				{
				_equality_expression = _EqualityExpression;
				}
}
}
