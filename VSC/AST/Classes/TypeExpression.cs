using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class TypeExpression : Semantic {
 			public PackageOrTypeExpr _package_or_type_expr;
			public PointerStars _pointer_stars;
			public BuiltinTypeExpression _builtin_type_expression;

			[Rule("<type expression> ::= <package or type expr>")]
			public TypeExpression(PackageOrTypeExpr _PackageOrTypeExpr)
				{
				_package_or_type_expr = _PackageOrTypeExpr;
				}
			[Rule("<type expression> ::= <package or type expr> '?'")]
			public TypeExpression(PackageOrTypeExpr _PackageOrTypeExpr, Semantic _symbol32)
				{
				_package_or_type_expr = _PackageOrTypeExpr;
				}
			[Rule("<type expression> ::= <package or type expr> <pointer stars>")]
			public TypeExpression(PackageOrTypeExpr _PackageOrTypeExpr,PointerStars _PointerStars)
				{
				_package_or_type_expr = _PackageOrTypeExpr;
				_pointer_stars = _PointerStars;
				}
			[Rule("<type expression> ::= <builtin type expression>")]
			public TypeExpression(BuiltinTypeExpression _BuiltinTypeExpression)
				{
				_builtin_type_expression = _BuiltinTypeExpression;
				}
}
}
