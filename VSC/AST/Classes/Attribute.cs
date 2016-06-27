using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class Attribute : Semantic {
 			public PackageOrTypeExpr _package_or_type_expr;
			public OptArgumentList _opt_argument_list;

			[Rule("<attribute> ::= <package or type expr> '(' <opt argument list> ')'")]
            public Attribute(PackageOrTypeExpr _PackageOrTypeExpr, Semantic _symbol20, OptArgumentList arglist, Semantic _symbol21)
				{
				_package_or_type_expr = _PackageOrTypeExpr;
                _opt_argument_list = arglist;
				}
	
			[Rule("<attribute> ::= <package or type expr>")]
			public Attribute(PackageOrTypeExpr _PackageOrTypeExpr)
				{
				_package_or_type_expr = _PackageOrTypeExpr;
				}
}
}
