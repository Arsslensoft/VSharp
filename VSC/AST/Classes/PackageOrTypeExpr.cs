using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class PackageOrTypeExpr :  Expression {
 			public QualifiedAliasMember _qualified_alias_member;
			public SimpleNameExpr _simple_name_expr;
			public PackageOrTypeExpr _package_or_type_expr;

			[Rule("<package or type expr> ::= <Qualified Alias Member>")]
			public PackageOrTypeExpr(QualifiedAliasMember _QualifiedAliasMember)
				{
				_qualified_alias_member = _QualifiedAliasMember;
				}
			[Rule("<package or type expr> ::= <simple name expr>")]
			public PackageOrTypeExpr(SimpleNameExpr _SimpleNameExpr)
				{
				_simple_name_expr = _SimpleNameExpr;
				}
			[Rule("<package or type expr> ::= <package or type expr> '.' <simple name expr>")]
			public PackageOrTypeExpr(PackageOrTypeExpr _PackageOrTypeExpr, Semantic _symbol25,SimpleNameExpr _SimpleNameExpr)
				{
				_package_or_type_expr = _PackageOrTypeExpr;
				_simple_name_expr = _SimpleNameExpr;
				}
}
}
