using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class GotoDefaultStatement : Statement
    {
 
			[Rule("<goto default statement> ::= goto default ';'")]
			public GotoDefaultStatement( Semantic _symbol102, Semantic _symbol86, Semantic _symbol31)
				{
				}
}
}
