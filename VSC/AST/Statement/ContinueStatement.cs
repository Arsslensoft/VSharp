using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ContinueStatement : Statement
    {
 
			[Rule("<continue statement> ::= continue ';'")]
			public ContinueStatement( Semantic _symbol84, Semantic _symbol31)
				{
				}
}
}
