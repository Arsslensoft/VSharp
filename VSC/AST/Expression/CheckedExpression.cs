using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class CheckedExpression : Expression
    {
 			public Expression _expression;

			[Rule("<checked expression> ::= checked '(' <expression> ')'")]
			public CheckedExpression( Semantic _symbol81, Semantic _symbol20,Expression _Expression, Semantic _symbol21)
				{
				_expression = _Expression;
				}
}
}
