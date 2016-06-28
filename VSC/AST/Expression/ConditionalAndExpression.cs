using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ConditionalAndExpression : BinaryExpression
    {
 		
			[Rule("<conditional and expression> ::= <conditional and expression> '&&' <inclusive or expression>")]
        public ConditionalAndExpression(Expression left, Semantic op, Expression right)
        {
            _left = left;
            _right = right;
        }
			
}
}
