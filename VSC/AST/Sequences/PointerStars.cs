using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class PointerStars : Sequence<Semantic> {
 			[Rule("<pointer stars> ::= '*'")]
			public PointerStars( Semantic _symbol22) : base(_symbol22)
				{
				}
			[Rule("<pointer stars> ::= <pointer stars> '*'")]
			public PointerStars(PointerStars _PointerStars, Semantic _symbol22) : base(_symbol22,_PointerStars)
				{
				
				}
}
}
