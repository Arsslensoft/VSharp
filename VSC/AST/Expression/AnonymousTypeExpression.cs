using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class AnonymousTypeExpression : Expression
    {
 			public AnonymousTypeParametersOptComma _anonymous_type_parameters_opt_comma;

			[Rule("<anonymous type expression> ::= new '{' <anonymous type parameters opt comma> '}'")]
			public AnonymousTypeExpression( Semantic _symbol115, Semantic _symbol43,AnonymousTypeParametersOptComma _AnonymousTypeParametersOptComma, Semantic _symbol47)
				{
				_anonymous_type_parameters_opt_comma = _AnonymousTypeParametersOptComma;
				}
}
}
