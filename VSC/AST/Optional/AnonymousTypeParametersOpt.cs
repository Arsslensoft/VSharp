using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class AnonymousTypeParametersOpt : Semantic {
 			public AnonymousTypeParameters _anonymous_type_parameters;

			[Rule("<anonymous type parameters opt> ::= ")]
			public AnonymousTypeParametersOpt()
				{
				}
			[Rule("<anonymous type parameters opt> ::= <anonymous type parameters>")]
			public AnonymousTypeParametersOpt(AnonymousTypeParameters _AnonymousTypeParameters)
				{
				_anonymous_type_parameters = _AnonymousTypeParameters;
				}
}
}
