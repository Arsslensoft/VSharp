using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ExclusiveOrExpression : BinaryExpression
    {
			[Rule("<exclusive or expression> ::= <exclusive or expression> '^' <and expression>")]
        public ExclusiveOrExpression(Expression left, Semantic op, Expression right)
        {
            _left = left;
            _right = right;
        }
			
}
}
