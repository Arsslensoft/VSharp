using System;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;
namespace VSC.AST { 
	public class PackageDeclaration : Declaration {
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

            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                DomRegion region = rc.MakeRegion(this, _opt_semicolon);
                UsingScope previousUsingScope = rc.usingScope;
                foreach (var ident in _package_or_type_expr.Identifiers)
                {
                    rc.usingScope = new UsingScope(rc.usingScope, ident);
                    rc.usingScope.Region = region;
                }

                if (_opt_attributes._attribute_sections != null)
                    rc.ConvertAttributes(rc.unresolvedFile.AssemblyAttributes,_opt_attributes._attribute_sections);


                foreach (ImportDirective imp in _import_directives)
                    imp.Resolve(rc);

                foreach (PackageOrTypeDeclaration ptd in _package_or_type_declarations)
                    ptd.Resolve(rc);


                rc.unresolvedFile.UsingScopes.Add(rc.usingScope); // add after visiting children so that nested scopes come first
                rc.usingScope = previousUsingScope;
                return true;
          
            }
}
}
