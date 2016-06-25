using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ShiftExpression : Expression
    {
 			public ShiftExpression _shift_expression;
			public AdditiveExpression _additive_expression;

			[Rule("<shift expression> ::= <shift expression> '<<' <additive expression>")]
			[Rule("<shift expression> ::= <shift expression> '>>' <additive expression>")]
			[Rule("<shift expression> ::= <shift expression> '<~' <additive expression>")]
			[Rule("<shift expression> ::= <shift expression> '~>' <additive expression>")]
			public ShiftExpression(ShiftExpression _ShiftExpression, Semantic _symbol57,AdditiveExpression _AdditiveExpression)
				{
				_shift_expression = _ShiftExpression;
				_additive_expression = _AdditiveExpression;
				}
			[Rule("<shift expression> ::= <additive expression>")]
			public ShiftExpression(AdditiveExpression _AdditiveExpression)
				{
				_additive_expression = _AdditiveExpression;
				}
}
}
