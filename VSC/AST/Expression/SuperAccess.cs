using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class SuperAccess : Expression {
 			public ExpressionList _expression_list;

			[Rule("<super access> ::= super '[' <expression list> ']'")]
			public SuperAccess( Semantic _symbol146, Semantic _symbol37,ExpressionList _ExpressionList, Semantic _symbol40)
				{
				_expression_list = _ExpressionList;
				}
}
}
