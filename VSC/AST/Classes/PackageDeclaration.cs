using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class PackageDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public PackageOrTypeExpr _package_or_type_expr;
			public ImportDirectives _import_directives;
			public PackageOrTypeDeclarations _package_or_type_declarations;
			public OptSemicolon _opt_semicolon;

			[Rule("<package declaration> ::= <opt attributes> package <package or type expr> '{' <import directives> <package or type declarations> '}' <opt semicolon>")]
			public PackageDeclaration(OptAttributes _OptAttributes, Semantic _symbol124,PackageOrTypeExpr _PackageOrTypeExpr, Semantic _symbol43,ImportDirectives _ImportDirectives,PackageOrTypeDeclarations _PackageOrTypeDeclarations, Semantic _symbol47,OptSemicolon _OptSemicolon)
				{
				_opt_attributes = _OptAttributes;
				_package_or_type_expr = _PackageOrTypeExpr;
				_import_directives = _ImportDirectives;
				_package_or_type_declarations = _PackageOrTypeDeclarations;
				_opt_semicolon = _OptSemicolon;
				}
}
}
