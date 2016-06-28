using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class InclusiveOrExpression : BinaryExpression
    {


        [Rule("<inclusive or expression> ::= <inclusive or expression> '|' <exclusive or expression>")]
        public InclusiveOrExpression(Expression left, Semantic op, Expression right)
        {
            _left = left;
            _right = right;
        }
    }
}
