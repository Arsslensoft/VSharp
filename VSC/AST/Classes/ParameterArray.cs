using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ParameterArray : Semantic {
 			public OptAttributes _opt_attributes;
			public Type _type;
			public Identifier _identifier;
			public Expression _expression;

			[Rule("<parameter array> ::= <opt attributes> params <type> <Identifier>")]
			public ParameterArray(OptAttributes _OptAttributes, Semantic _symbol125,Type _Type,Identifier _Identifier)
				{
				_opt_attributes = _OptAttributes;
				_type = _Type;
				_identifier = _Identifier;
				}
			[Rule("<parameter array> ::= <opt attributes> params <type> <Identifier> '=' <expression>")]
			public ParameterArray(OptAttributes _OptAttributes, Semantic _symbol125,Type _Type,Identifier _Identifier, Semantic _symbol60,Expression _Expression)
				{
				_opt_attributes = _OptAttributes;
				_type = _Type;
				_identifier = _Identifier;
				_expression = _Expression;
				}
}
}
