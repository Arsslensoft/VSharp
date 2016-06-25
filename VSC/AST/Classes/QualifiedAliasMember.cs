using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class QualifiedAliasMember : Semantic {
 			public Identifier _identifier;
			public SimpleNameExpr _simple_name_expr;

			[Rule("<Qualified Alias Member> ::= <Identifier> '::' <simple name expr>")]
			public QualifiedAliasMember(Identifier _Identifier, Semantic _symbol29,SimpleNameExpr _SimpleNameExpr)
				{
				_identifier = _Identifier;
				_simple_name_expr = _SimpleNameExpr;
				}
}
}
