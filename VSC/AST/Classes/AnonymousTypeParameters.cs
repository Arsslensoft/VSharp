using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class AnonymousTypeParameters : Semantic {
 			public AnonymousTypeParameter _anonymous_type_parameter;
			public AnonymousTypeParameters _anonymous_type_parameters;

			[Rule("<anonymous type parameters> ::= <anonymous type parameter>")]
			public AnonymousTypeParameters(AnonymousTypeParameter _AnonymousTypeParameter)
				{
				_anonymous_type_parameter = _AnonymousTypeParameter;
				}
			[Rule("<anonymous type parameters> ::= <anonymous type parameters> ',' <anonymous type parameter>")]
			public AnonymousTypeParameters(AnonymousTypeParameters _AnonymousTypeParameters, Semantic _symbol24,AnonymousTypeParameter _AnonymousTypeParameter)
				{
				_anonymous_type_parameters = _AnonymousTypeParameters;
				_anonymous_type_parameter = _AnonymousTypeParameter;
				}
}
}
