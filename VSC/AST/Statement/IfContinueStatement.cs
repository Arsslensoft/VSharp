using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class IfContinueStatement : Statement
    {
 
			[Rule("<if continue statement> ::= if continue ';'")]
			public IfContinueStatement( Semantic _symbol105, Semantic _symbol84, Semantic _symbol31)
				{
				}
}
}
