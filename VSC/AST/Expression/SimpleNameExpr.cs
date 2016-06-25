using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class SimpleNameExpr : Expression
    {
 			public Identifier _identifier;
			public OptTypeArgumentList _opt_type_argument_list;

			[Rule("<simple name expr> ::= <Identifier> <opt type argument list>")]
			public SimpleNameExpr(Identifier _Identifier,OptTypeArgumentList _OptTypeArgumentList)
				{
				_identifier = _Identifier;
				_opt_type_argument_list = _OptTypeArgumentList;
				}
}
}
