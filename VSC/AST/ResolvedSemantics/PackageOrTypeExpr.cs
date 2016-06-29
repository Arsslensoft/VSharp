using System;
using System.Linq;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class PackageOrTypeExpr :  Expression {
 			public QualifiedAliasMember _qualified_alias_member;
			public SimpleNameExpr _simple_name_expr;
			public PackageOrTypeExpr _package_or_type_expr;

            public string[] Identifiers
            {
                get
                {
                    if (_qualified_alias_member != null)
                        return new string[2] { _qualified_alias_member._identifier._Identifier, _qualified_alias_member._simple_name_expr._identifier._Identifier };
                    else if (_package_or_type_expr != null)
                        return (string[])_package_or_type_expr.Identifiers.Concat(new string[1] { _simple_name_expr._identifier._Identifier });
                    else
                        return new string[1] { _simple_name_expr._identifier._Identifier };
                }
            }
            public override string ToString()
            {
                if (_qualified_alias_member != null)
                    return _qualified_alias_member._identifier._Identifier + "::" + _qualified_alias_member._simple_name_expr.ToString();
                else if (_package_or_type_expr != null)
                    return _package_or_type_expr.ToString() + "." + _simple_name_expr.ToString();
                else return _simple_name_expr.ToString();
            }
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
