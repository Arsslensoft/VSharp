using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ImportDirective : Semantic {
 			public Identifier _identifier;
			public PackageOrTypeExpr _package_or_type_expr;

			[Rule("<import directive> ::= import <Identifier> '=' <package or type expr> ';'")]
			public ImportDirective( Semantic _symbol107,Identifier _Identifier, Semantic _symbol60,PackageOrTypeExpr _PackageOrTypeExpr, Semantic _symbol31)
				{
				_identifier = _Identifier;
				_package_or_type_expr = _PackageOrTypeExpr;
				}
			[Rule("<import directive> ::= import <package or type expr> ';'")]
			public ImportDirective( Semantic _symbol107,PackageOrTypeExpr _PackageOrTypeExpr, Semantic _symbol31)
				{
				_package_or_type_expr = _PackageOrTypeExpr;
				}
}
}
