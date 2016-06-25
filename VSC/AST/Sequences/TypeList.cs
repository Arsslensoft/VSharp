using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class TypeList : Sequence<Type> {
 			

			[Rule("<type list> ::= <type>")]
			public TypeList(Type _Type) : base(_Type)
				{
				
				}
			[Rule("<type list> ::= <type list> ',' <type>")]
			public TypeList(TypeList _TypeList, Semantic _symbol24,Type _Type) : base(_Type,_TypeList)
				{
				
				}
}
}
