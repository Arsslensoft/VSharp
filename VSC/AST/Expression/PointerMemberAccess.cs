using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class PointerMemberAccess : Expression {
 			public Expression _primary_expression;
			public SimpleNameExpr _simple_name_expr;

			[Rule("<pointer member access> ::= <primary expression> '->' <simple name expr>")]
			public PointerMemberAccess(Expression _PrimaryExpression, Semantic _symbol65,SimpleNameExpr _SimpleNameExpr)
				{
				_primary_expression = _PrimaryExpression;
				_simple_name_expr = _SimpleNameExpr;
				}
}
}
