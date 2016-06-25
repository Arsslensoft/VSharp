using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class NegationExpression : Expression
    {
 			public Expression _unary_expression;

			[Rule("<negation expression> ::= '-' <unary expression>")]
			public NegationExpression( Semantic _symbol8,Expression _UnaryExpression)
				{
				_unary_expression = _UnaryExpression;
				}
}
}
