using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class Modifiers : Sequence<Modifier> {
			[Rule("<modifiers> ::= <modifier>")]
			public Modifiers(Modifier _Modifier) : base(_Modifier)
				{
				
				}
			[Rule("<modifiers> ::= <modifiers> <modifier>")]
			public Modifiers(Modifiers _Modifiers,Modifier _Modifier) : base(_Modifier,_Modifiers)
				{
				
				}
}
}
