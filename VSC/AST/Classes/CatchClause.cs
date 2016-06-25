using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class CatchClause : Semantic {
 			public PackageOrTypeExpr _package_or_type_expr;
			public Identifier _identifier;
			public BlockStatement _block_statement;

			[Rule("<Catch Clause> ::= catch '(' <package or type expr> <Identifier> ')' <block statement>")]
			public CatchClause( Semantic _symbol78, Semantic _symbol20,PackageOrTypeExpr _PackageOrTypeExpr,Identifier _Identifier, Semantic _symbol21,BlockStatement _BlockStatement)
				{
				_package_or_type_expr = _PackageOrTypeExpr;
				_identifier = _Identifier;
				_block_statement = _BlockStatement;
				}
			[Rule("<Catch Clause> ::= catch <block statement>")]
			public CatchClause( Semantic _symbol78,BlockStatement _BlockStatement)
				{
				_block_statement = _BlockStatement;
				}
}
}
