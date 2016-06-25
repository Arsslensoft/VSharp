using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class GotoCaseStatement : Statement
    {
 			public Expression _expression;

			[Rule("<goto case statement> ::= goto case <expression> ';'")]
			public GotoCaseStatement( Semantic _symbol102, Semantic _symbol77,Expression _Expression, Semantic _symbol31)
				{
				_expression = _Expression;
				}
}
}
