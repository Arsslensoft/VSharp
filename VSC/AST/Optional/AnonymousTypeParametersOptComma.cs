using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class AnonymousTypeParametersOptComma : Semantic {
 			public AnonymousTypeParametersOpt _anonymous_type_parameters_opt;
			public AnonymousTypeParameters _anonymous_type_parameters;

			[Rule("<anonymous type parameters opt comma> ::= <anonymous type parameters opt>")]
			public AnonymousTypeParametersOptComma(AnonymousTypeParametersOpt _AnonymousTypeParametersOpt)
				{
				_anonymous_type_parameters_opt = _AnonymousTypeParametersOpt;
				}
			[Rule("<anonymous type parameters opt comma> ::= <anonymous type parameters> ','")]
			public AnonymousTypeParametersOptComma(AnonymousTypeParameters _AnonymousTypeParameters, Semantic _symbol24)
				{
				_anonymous_type_parameters = _AnonymousTypeParameters;
				}
}
}
