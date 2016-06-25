using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ExpressionList : Sequence<Expression> {
 		
			[Rule("<expression list> ::= <expression>")]
			public ExpressionList(Expression _Expression) : base(_Expression)
				{
				
				}
			[Rule("<expression list> ::= <expression list> ',' <expression>")]
			public ExpressionList(ExpressionList _ExpressionList, Semantic _symbol24,Expression _Expression) : base(_Expression,_ExpressionList)
				{
				}
}
}
