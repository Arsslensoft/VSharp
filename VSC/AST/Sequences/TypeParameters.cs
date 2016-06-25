using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class TypeParameters : Sequence<Identifier> {
 		
			[Rule("<type parameters> ::= <Identifier>")]
			public TypeParameters(Identifier _Identifier) : base(_Identifier)
				{
				
				}
			[Rule("<type parameters> ::= <type parameters> ',' <Identifier>")]
			public TypeParameters(TypeParameters _TypeParameters, Semantic _symbol24,Identifier _Identifier) : base(_Identifier,_TypeParameters)
				{
				
				
				}
}
}
