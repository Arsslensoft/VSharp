using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ShiftExpression : BinaryExpression
    {


			[Rule("<shift expression> ::= <shift expression> '<<' <additive expression>")]
			[Rule("<shift expression> ::= <shift expression> '>>' <additive expression>")]
			[Rule("<shift expression> ::= <shift expression> '<~' <additive expression>")]
			[Rule("<shift expression> ::= <shift expression> '~>' <additive expression>")]
            public ShiftExpression(Expression left, Semantic op, Expression right)
				{
				_left = left;
                _right = right;
				}
		
}
}
