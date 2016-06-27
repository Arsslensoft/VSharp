using System;
using System.Collections.Generic;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;
namespace VSC.AST {
    public class ImportDirective : ResolvableSemantic
    {
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

            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                if (_identifier == null)
                {
                    var u = rc.ConvertTypeReference(_package_or_type_expr, NameLookupMode.TypeInUsingDeclaration) as TypeOrNamespaceReference;
                    if (u != null)
                    {
                        rc.usingScope.Usings.Add(u);
                        return true;
                    }
                    else
                    {
                        rc.Compiler.Report.Error(1, Location,"The namespace or type {0} is does not seem to exist or it was not imported correctly", _package_or_type_expr.ToString());
                        return false;
                    }
                }
                else
                {
                    TypeOrNamespaceReference u = rc.ConvertTypeReference(_package_or_type_expr, NameLookupMode.TypeInUsingDeclaration) as TypeOrNamespaceReference;
                    if (u != null)
                    {
                        rc.usingScope.UsingAliases.Add(new KeyValuePair<string, TypeOrNamespaceReference>(_identifier._Identifier, u));
                        return true;
                    }
                    else
                    {
                        rc.Compiler.Report.Error(1, Location, "The namespace or type {0} is does not seem to exist or it was not imported correctly", _package_or_type_expr.ToString());
                        return false;
                    }
                }
    
            }
  
}
}
