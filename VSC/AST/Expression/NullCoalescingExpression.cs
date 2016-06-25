using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class NullCoalescingExpression : Expression
    {
 			public NullCoalescingExpression _null_coalescing_expression;
			public BinaryOperationExpression _binary_operation_expression;

			[Rule("<null coalescing expression> ::= <null coalescing expression> '??' <Binary Operation Expression>")]
			public NullCoalescingExpression(NullCoalescingExpression _NullCoalescingExpression, Semantic _symbol35,BinaryOperationExpression _BinaryOperationExpression)
				{
				_null_coalescing_expression = _NullCoalescingExpression;
				_binary_operation_expression = _BinaryOperationExpression;
				}
			[Rule("<null coalescing expression> ::= <Binary Operation Expression>")]
			public NullCoalescingExpression(BinaryOperationExpression _BinaryOperationExpression)
				{
				_binary_operation_expression = _BinaryOperationExpression;
				}
}
}
