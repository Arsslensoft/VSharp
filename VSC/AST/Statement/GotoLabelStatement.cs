using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class GotoLabelStatement : Statement
    {
 			public Identifier _identifier;

			[Rule("<goto label statement> ::= goto <Identifier> ';'")]
			public GotoLabelStatement( Semantic _symbol102,Identifier _Identifier, Semantic _symbol31)
				{
				_identifier = _Identifier;
				}
}
}
