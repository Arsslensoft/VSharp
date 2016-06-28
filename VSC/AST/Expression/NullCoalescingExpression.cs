using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class NullCoalescingExpression : BinaryExpression
    {
 			
			[Rule("<null coalescing expression> ::= <null coalescing expression> '??' <Binary Operation Expression>")]
        public NullCoalescingExpression(Expression left, Semantic op, Expression right)
        {
            _left = left;
            _right = right;
        }
}
}
