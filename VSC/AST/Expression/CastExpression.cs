using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class CastExpression : Expression
    {
 			public Type _type;
			public Expression _prefixed_unary_expression;

			[Rule("<cast expression> ::= '$(' <type> ')' <prefixed unary expression>")]
			public CastExpression( Semantic _symbol14,Type _Type, Semantic _symbol21,Expression _PrefixedUnaryExpression)
				{
				_type = _Type;
				_prefixed_unary_expression = _PrefixedUnaryExpression;
				}
}
}
