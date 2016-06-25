using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class OnesComplementExpression : Expression
    {
 			public Expression _prefixed_unary_expression;

			[Rule("<ones complement expression> ::= '~' <prefixed unary expression>")]
			public OnesComplementExpression( Semantic _symbol48,Expression _PrefixedUnaryExpression)
				{
				_prefixed_unary_expression = _PrefixedUnaryExpression;
				}
}
}
