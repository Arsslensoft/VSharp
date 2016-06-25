using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ExpressionBlock : Semantic {
 			public Expression _expression;

			[Rule("<expression block> ::= '=>' <expression> ';'")]
			public ExpressionBlock( Semantic _symbol63,Expression _Expression, Semantic _symbol31)
				{
				_expression = _Expression;
				}
}
}
