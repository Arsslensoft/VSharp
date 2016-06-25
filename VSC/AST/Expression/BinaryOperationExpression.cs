using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class BinaryOperationExpression : Expression
    {
 			public BinaryOperationExpression _binary_operation_expression;
			public BinaryOperatorLiteral _binary_operator_constant;
			public ConditionalOrExpression _conditional_or_expression;

			[Rule("<Binary Operation Expression> ::= <Binary Operation Expression> <Binary Operator Constant> <conditional or expression>")]
			public BinaryOperationExpression(BinaryOperationExpression _BinaryOperationExpression,BinaryOperatorLiteral _BinaryOperatorConstant,ConditionalOrExpression _ConditionalOrExpression)
				{
				_binary_operation_expression = _BinaryOperationExpression;
				_binary_operator_constant = _BinaryOperatorConstant;
				_conditional_or_expression = _ConditionalOrExpression;
				}
			[Rule("<Binary Operation Expression> ::= <conditional or expression>")]
			public BinaryOperationExpression(ConditionalOrExpression _ConditionalOrExpression)
				{
				_conditional_or_expression = _ConditionalOrExpression;
				}
}
}
