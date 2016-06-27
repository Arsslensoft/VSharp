using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class FieldDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public MemberType _member_type;
			public Identifier _identifier;
			public OptFieldInitializer _opt_field_initializer;
			public OptFieldDeclarators _opt_field_declarators;

			[Rule("<field declaration> ::= <opt attributes> <opt modifiers> <member type> <Identifier> <opt field initializer> <opt field declarators> ';'")]
			public FieldDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers,MemberType _MemberType,Identifier _Identifier,OptFieldInitializer _OptFieldInitializer,OptFieldDeclarators _OptFieldDeclarators, Semantic _symbol31)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_member_type = _MemberType;
				_identifier = _Identifier;
				_opt_field_initializer = _OptFieldInitializer;
				_opt_field_declarators = _OptFieldDeclarators;
				}
}
}
