using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptSemicolon : Semantic {
 
			[Rule("<opt semicolon> ::= ';'")]
			public OptSemicolon( Semantic _symbol31)
				{
				}
			[Rule("<opt semicolon> ::= ")]
			public OptSemicolon()
				{
				}
}
}
