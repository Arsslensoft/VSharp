using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class FixedParameter : Semantic {
 			public OptAttributes _opt_attributes;
			public OptParameterModifier _opt_parameter_modifier;
			public Type _type;
			public Identifier _identifier;
			public Expression _expression;

			[Rule("<fixed parameter> ::= <opt attributes> <opt parameter modifier> <type> <Identifier>")]
            public FixedParameter(OptAttributes _OptAttributes, OptParameterModifier _OptParameterModifier, Type _Type, Identifier _Identifier)
				{
				_opt_attributes = _OptAttributes;
				_opt_parameter_modifier = _OptParameterModifier;
			    _type = _Type;
				_identifier = _Identifier;
				}
			[Rule("<fixed parameter> ::= <opt attributes> <opt parameter modifier> <type> <Identifier> '=' <expression>")]
			public FixedParameter(OptAttributes _OptAttributes,OptParameterModifier _OptParameterModifier,Type _Type,Identifier _Identifier, Semantic _symbol60,Expression _Expression)
				{
				_opt_attributes = _OptAttributes;
				_opt_parameter_modifier = _OptParameterModifier;
                _type = _Type;
				_identifier = _Identifier;
				_expression = _Expression;
				}
}
}
