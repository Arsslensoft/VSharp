using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class MultiplicativeExpression : Expression
    {
 			public MultiplicativeExpression _multiplicative_expression;
			public Expression _prefixed_unary_expression;

			[Rule("<multiplicative expression> ::= <multiplicative expression> '*' <prefixed unary expression>")]
			[Rule("<multiplicative expression> ::= <multiplicative expression> '/' <prefixed unary expression>")]
			[Rule("<multiplicative expression> ::= <multiplicative expression> '%' <prefixed unary expression>")]
			public MultiplicativeExpression(MultiplicativeExpression _MultiplicativeExpression, Semantic _symbol22,Expression _PrefixedUnaryExpression)
				{
				_multiplicative_expression = _MultiplicativeExpression;
				_prefixed_unary_expression = _PrefixedUnaryExpression;
				}
			[Rule("<multiplicative expression> ::= <prefixed unary expression>")]
			public MultiplicativeExpression(Expression _PrefixedUnaryExpression)
				{
				_prefixed_unary_expression = _PrefixedUnaryExpression;
				}
}
}
