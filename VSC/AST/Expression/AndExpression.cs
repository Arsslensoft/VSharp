using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class AndExpression : BinaryExpression
    {
    

			[Rule("<and expression> ::= <and expression> '&' <equality expression>")]
        public AndExpression(Expression left, Semantic op, Expression right)
        {
            _left = left;
            _right = right;
        }
		
}
}
