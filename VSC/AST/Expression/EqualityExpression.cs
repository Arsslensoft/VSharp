using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class EqualityExpression : BinaryExpression
    {
 	

			[Rule("<equality expression> ::= <equality expression> '==' <relational expression>")]
			[Rule("<equality expression> ::= <equality expression> '!=' <relational expression>")]
        public EqualityExpression(Expression left, Semantic op, Expression right)
        {
            _left = left;
            _right = right;
        }
}
}
