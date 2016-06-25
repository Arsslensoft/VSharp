using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class TypeArguments : Sequence<Type> {
 			[Rule("<type arguments> ::= <type>")]
			public TypeArguments(Type _Type) : base(_Type)
				{
				
				}
			[Rule("<type arguments> ::= <type arguments> ',' <type>")]
			public TypeArguments(TypeArguments _TypeArguments, Semantic _symbol24,Type _Type) : base(_Type)
				{
				
				}
}
}
