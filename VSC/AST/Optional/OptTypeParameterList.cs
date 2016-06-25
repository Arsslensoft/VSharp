using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptTypeParameterList : Semantic {
 			public TypeParameters _type_parameters;

			[Rule("<opt type parameter list> ::= ")]
			public OptTypeParameterList()
				{
				}
			[Rule("<opt type parameter list> ::= '<' <type parameters> '>'")]
			public OptTypeParameterList( Semantic _symbol54,TypeParameters _TypeParameters, Semantic _symbol64)
				{
				_type_parameters = _TypeParameters;
				}
}
}
