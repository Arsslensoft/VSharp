using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ConditionalExpression : Expression
    {
 			public NullCoalescingExpression _null_coalescing_expression;
			public Expression _fexpression;
            public Expression _texpression;
			[Rule("<conditional expression> ::= <null coalescing expression>")]
			public ConditionalExpression(NullCoalescingExpression _NullCoalescingExpression)
				{
				_null_coalescing_expression = _NullCoalescingExpression;
				}
			[Rule("<conditional expression> ::= <null coalescing expression> '?' <expression> ':' <expression>")]
			public ConditionalExpression(NullCoalescingExpression _NullCoalescingExpression, Semantic _symbol32,Expression _tExpression, Semantic _symbol28,Expression _fExpression)
				{
				_null_coalescing_expression = _NullCoalescingExpression;
				_texpression = _tExpression;
				_fexpression = _fExpression;
				}
}
}
