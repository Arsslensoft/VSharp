using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class EmptyStatement : Statement
    {
 
			[Rule("<empty statement> ::= ';'")]
			public EmptyStatement( Semantic _symbol31)
				{
				}
}
}
