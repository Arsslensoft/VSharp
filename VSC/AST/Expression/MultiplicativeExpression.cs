using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class MultiplicativeExpression : BinaryExpression
    {
 			
			[Rule("<multiplicative expression> ::= <multiplicative expression> '*' <prefixed unary expression>")]
			[Rule("<multiplicative expression> ::= <multiplicative expression> '/' <prefixed unary expression>")]
			[Rule("<multiplicative expression> ::= <multiplicative expression> '%' <prefixed unary expression>")]
        public MultiplicativeExpression(Expression left, Semantic op, Expression right)
        {
            _left = left;
            _right = right;
        }
			
}
}
