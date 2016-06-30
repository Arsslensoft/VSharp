using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class CompilationUnit : ResolvableSemantic {
 			public ImportDirectives _import_directives;
			public PackageDeclarations _package_declarations;
        
			[Rule("<compilation unit> ::= <import directives> <package declarations>")]
			public CompilationUnit(ImportDirectives _ImportDirectives,PackageDeclarations _PackageDeclarations)
				{
				_import_directives = _ImportDirectives;
				_package_declarations = _PackageDeclarations;
				}

            public override bool Resolve(Context.SymbolResolveContext rc)
            {

                foreach (ImportDirective imp in _import_directives)
                    imp.Resolve(rc);
                      

                foreach (PackageDeclaration pdecl in _package_declarations)
                   pdecl.Resolve(rc);


                return true;
            }
            public override object DoResolve(Context.ResolveContext rc)
            {
                foreach (ImportDirective imp in _import_directives)
                    imp.DoResolve(rc);


                foreach (PackageDeclaration pdecl in _package_declarations)
                    pdecl.DoResolve(rc);


                return this;
            }
}
}
