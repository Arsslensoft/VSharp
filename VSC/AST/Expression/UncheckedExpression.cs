using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class UncheckedExpression : Expression
    {
 			public Expression _expression;

			[Rule("<unchecked expression> ::= unchecked '(' <expression> ')'")]
			public UncheckedExpression( Semantic _symbol155, Semantic _symbol20,Expression _Expression, Semantic _symbol21)
				{
				_expression = _Expression;
				}
}
}
