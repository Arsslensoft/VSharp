using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class RelationalExpression : BinaryExpression
    {
 		
			[Rule("<relational expression> ::= <relational expression> '<' <shift expression>")]
			[Rule("<relational expression> ::= <relational expression> '>' <shift expression>")]
			[Rule("<relational expression> ::= <relational expression> '<=' <shift expression>")]
			[Rule("<relational expression> ::= <relational expression> '>=' <shift expression>")]
        public RelationalExpression(Expression left, Semantic op, Expression right)
        {
            _left = left;
            _right = right;
        }
}
}
