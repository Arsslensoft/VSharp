using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class MemberAccess : Expression
    {
        public Expression _accessible_primary_expression;
			public Identifier _identifier;
			public OptTypeArgumentList _opt_type_argument_list;
			public PackageOrTypeExpr _package_or_type_expr;

			[Rule("<member access> ::= <accessible primary expression> '.' <Identifier> <opt type argument list>")]
			[Rule("<member access> ::= <accessible primary expression> '?.' <Identifier> <opt type argument list>")]
			public MemberAccess(Expression _AccessiblePrimaryExpression, Semantic _symbol25,Identifier _Identifier,OptTypeArgumentList _OptTypeArgumentList)
				{
				_accessible_primary_expression = _AccessiblePrimaryExpression;
				_identifier = _Identifier;
				_opt_type_argument_list = _OptTypeArgumentList;
				}
			[Rule("<member access> ::= <package or type expr>")]
			public MemberAccess(PackageOrTypeExpr _PackageOrTypeExpr)
				{
				_package_or_type_expr = _PackageOrTypeExpr;
				}
        [Rule("<member access> ::= super '?.' <Identifier> <opt type argument list>")]	
        [Rule("<member access> ::= super '.' <Identifier> <opt type argument list>")]
			public MemberAccess( Semantic _symbol146, Semantic _symbol25,Identifier _Identifier,OptTypeArgumentList _OptTypeArgumentList)
				{
				_identifier = _Identifier;
				_opt_type_argument_list = _OptTypeArgumentList;
				}
			
			
}
}
