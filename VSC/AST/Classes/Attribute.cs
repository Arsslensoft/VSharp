using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class Attribute : Semantic {
 			public PackageOrTypeExpr _package_or_type_expr;
			public ExpressionList _expression_list;

			[Rule("<attribute> ::= <package or type expr> '(' <expression list> ')'")]
			public Attribute(PackageOrTypeExpr _PackageOrTypeExpr, Semantic _symbol20,ExpressionList _ExpressionList, Semantic _symbol21)
				{
				_package_or_type_expr = _PackageOrTypeExpr;
				_expression_list = _ExpressionList;
				}
			[Rule("<attribute> ::= <package or type expr> '(' ')'")]
			public Attribute(PackageOrTypeExpr _PackageOrTypeExpr, Semantic _symbol20, Semantic _symbol21)
				{
				_package_or_type_expr = _PackageOrTypeExpr;
				}
			[Rule("<attribute> ::= <package or type expr>")]
			public Attribute(PackageOrTypeExpr _PackageOrTypeExpr)
				{
				_package_or_type_expr = _PackageOrTypeExpr;
				}
}
}
