using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class BreakStatement : Statement
    {
 			public Expression _expression;

			[Rule("<break statement> ::= break <expression> ';'")]
			public BreakStatement( Semantic _symbol75,Expression _Expression, Semantic _symbol31)
				{
				_expression = _Expression;
				}
			[Rule("<break statement> ::= break ';'")]
			public BreakStatement( Semantic _symbol75, Semantic _symbol31)
				{
				}
}
}
