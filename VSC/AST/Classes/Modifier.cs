using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class Modifier : Semantic {
 
			[Rule("<modifier> ::= abstract")]
			[Rule("<modifier> ::= extern")]
			[Rule("<modifier> ::= new")]
			[Rule("<modifier> ::= override")]
			[Rule("<modifier> ::= sync")]
			[Rule("<modifier> ::= readonly")]
			[Rule("<modifier> ::= sealed")]
			[Rule("<modifier> ::= static")]
			[Rule("<modifier> ::= unsafe")]
			[Rule("<modifier> ::= virtual")]
			[Rule("<modifier> ::= private")]
			[Rule("<modifier> ::= protected")]
			[Rule("<modifier> ::= public")]
			[Rule("<modifier> ::= internal")]
			public Modifier( Semantic _symbol69)
				{
				}
}
}
