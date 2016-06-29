using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class Modifier : Semantic {

        public VSC.TypeSystem.Modifiers _Modifier;
			[Rule("<modifier> ::= abstract")]
			[Rule("<modifier> ::= extern")]
			[Rule("<modifier> ::= new")]
			[Rule("<modifier> ::= override")]
			[Rule("<modifier> ::= sync")]
			[Rule("<modifier> ::= readonly")]
			[Rule("<modifier> ::= sealed")]
			[Rule("<modifier> ::= static")]
			[Rule("<modifier> ::= supersede")]
			[Rule("<modifier> ::= virtual")]
			[Rule("<modifier> ::= private")]
			[Rule("<modifier> ::= protected")]
			[Rule("<modifier> ::= public")]
			[Rule("<modifier> ::= internal")]
			public Modifier( Semantic _symbol69)
				{
                    _Modifier = (VSC.TypeSystem.Modifiers)Enum.Parse(typeof(VSC.TypeSystem.Modifiers), _symbol69.Name.ToUpper());
				}
}
}
