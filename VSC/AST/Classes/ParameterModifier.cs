using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ParameterModifier : Semantic {
 
			[Rule("<parameter modifier> ::= ref")]
			[Rule("<parameter modifier> ::= out")]
			[Rule("<parameter modifier> ::= self")]
			public ParameterModifier( Semantic _symbol132)
				{
				}
}
}