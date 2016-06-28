using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class AdditiveExpression : BinaryExpression
    {

			[Rule("<additive expression> ::= <additive expression> '+' <multiplicative expression>")]
			[Rule("<additive expression> ::= <additive expression> '-' <multiplicative expression>")]
            public AdditiveExpression(Expression left, Semantic op, Expression right)
            {
                _left = left;
                _right = right;
            }
            [Rule("<additive expression> ::= <additive expression> is <type>")]
			[Rule("<additive expression> ::= <additive expression> as <type>")]
            public AdditiveExpression(Expression left, Semantic op, Type right)
            {
                _left = left;
                _right_type = right;
            }
}
}
