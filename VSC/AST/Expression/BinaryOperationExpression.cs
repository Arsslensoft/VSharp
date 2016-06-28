using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class BinaryOperationExpression : BinaryExpression
    {

			public BinaryOperatorLiteral _binary_operator_constant;


			[Rule("<Binary Operation Expression> ::= <Binary Operation Expression> <Binary Operator Constant> <conditional or expression>")]
            public BinaryOperationExpression(Expression left, BinaryOperatorLiteral op, Expression right)
            {
                _left = left; _binary_operator_constant = op;
                _right = right;
            }
	
}
}
