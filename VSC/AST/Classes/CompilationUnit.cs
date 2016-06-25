using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class CompilationUnit : Semantic {
 			public ImportDirectives _import_directives;
			public PackageDeclarations _package_declarations;

			[Rule("<compilation unit> ::= <import directives> <package declarations>")]
			public CompilationUnit(ImportDirectives _ImportDirectives,PackageDeclarations _PackageDeclarations)
				{
				_import_directives = _ImportDirectives;
				_package_declarations = _PackageDeclarations;
				}
}
}
