using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ConditionalOrExpression : BinaryExpression
    {
 			
			[Rule("<conditional or expression> ::= <conditional or expression> '||' <conditional and expression>")]
        public ConditionalOrExpression(Expression left, Semantic op, Expression right)
        {
            _left = left;
            _right = right;
        }
		
}
}
