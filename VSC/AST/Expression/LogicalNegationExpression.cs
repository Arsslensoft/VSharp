using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class LogicalNegationExpression : Expression
    {
 			public Expression _prefixed_unary_expression;

			[Rule("<logical negation expression> ::= '!' <prefixed unary expression>")]
			public LogicalNegationExpression( Semantic _symbol10,Expression _PrefixedUnaryExpression)
				{
				_prefixed_unary_expression = _PrefixedUnaryExpression;
				}
}
}
