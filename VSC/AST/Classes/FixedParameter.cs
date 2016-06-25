using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class FixedParameter : Semantic {
 			public OptAttributes _opt_attributes;
			public OptParameterModifier _opt_parameter_modifier;
			public MemberType _member_type;
			public Identifier _identifier;
			public Expression _expression;

			[Rule("<fixed parameter> ::= <opt attributes> <opt parameter modifier> <member type> <Identifier>")]
			public FixedParameter(OptAttributes _OptAttributes,OptParameterModifier _OptParameterModifier,MemberType _MemberType,Identifier _Identifier)
				{
				_opt_attributes = _OptAttributes;
				_opt_parameter_modifier = _OptParameterModifier;
				_member_type = _MemberType;
				_identifier = _Identifier;
				}
			[Rule("<fixed parameter> ::= <opt attributes> <opt parameter modifier> <member type> <Identifier> '=' <expression>")]
			public FixedParameter(OptAttributes _OptAttributes,OptParameterModifier _OptParameterModifier,MemberType _MemberType,Identifier _Identifier, Semantic _symbol60,Expression _Expression)
				{
				_opt_attributes = _OptAttributes;
				_opt_parameter_modifier = _OptParameterModifier;
				_member_type = _MemberType;
				_identifier = _Identifier;
				_expression = _Expression;
				}
}
}
