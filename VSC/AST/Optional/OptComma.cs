using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptComma : Semantic {
 
			[Rule("<opt comma> ::= ','")]
			public OptComma( Semantic _symbol24)
				{
				}
			[Rule("<opt comma> ::= ")]
			public OptComma()
				{
				}
}
}
