using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ElementAccess : Expression {
 			public Expression _primary_expression;
			public ExpressionList _expression_list;

			[Rule("<element access> ::= <primary expression> '[' <expression list> ']'")]
			[Rule("<element access> ::= <primary expression> '?[' <expression list> ']'")]
			public ElementAccess(Expression _PrimaryExpression, Semantic _symbol37,ExpressionList _ExpressionList, Semantic _symbol40)
				{
				_primary_expression = _PrimaryExpression;
				_expression_list = _ExpressionList;
				}
}
}
