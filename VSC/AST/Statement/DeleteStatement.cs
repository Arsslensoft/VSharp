using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class DeleteStatement : Statement
    {
 			public Expression _expression;

			[Rule("<delete statement> ::= delete <expression> ';'")]
			public DeleteStatement( Semantic _symbol88,Expression _Expression, Semantic _symbol31)
				{
				_expression = _Expression;
				}
}
}
