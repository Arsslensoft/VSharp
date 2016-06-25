using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class EnumMemberDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public Identifier _identifier;
			public Expression _expression;
			public AttributeSections _attribute_sections;

			[Rule("<enum member declaration> ::= <opt attributes> <Identifier>")]
			public EnumMemberDeclaration(OptAttributes _OptAttributes,Identifier _Identifier)
				{
				_opt_attributes = _OptAttributes;
				_identifier = _Identifier;
				}
			[Rule("<enum member declaration> ::= <opt attributes> <Identifier> '=' <expression>")]
			public EnumMemberDeclaration(OptAttributes _OptAttributes,Identifier _Identifier, Semantic _symbol60,Expression _Expression)
				{
				_opt_attributes = _OptAttributes;
				_identifier = _Identifier;
				_expression = _Expression;
				}
			[Rule("<enum member declaration> ::= <attribute sections>")]
			public EnumMemberDeclaration(AttributeSections _AttributeSections)
				{
				_attribute_sections = _AttributeSections;
				}
}
}
