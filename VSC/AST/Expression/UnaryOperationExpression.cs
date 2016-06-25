using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class UnaryOperationExpression : Expression
    {
 			public UnaryOperatorLiteral _unary_operator_constant;
			public Expression _unary_expression;

			[Rule("<unary operation expression> ::= <Unary Operator Constant> <unary expression>")]
			public UnaryOperationExpression(UnaryOperatorLiteral _UnaryOperatorConstant,Expression _UnaryExpression)
				{
				_unary_operator_constant = _UnaryOperatorConstant;
				_unary_expression = _UnaryExpression;
				}
}
}
