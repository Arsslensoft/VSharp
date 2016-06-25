using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class EnumDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public TypeDeclarationName _type_declaration_name;
			public OptEnumBase _opt_enum_base;
			public OptEnumMemberDeclarations _opt_enum_member_declarations;
			public OptSemicolon _opt_semicolon;

			[Rule("<enum declaration> ::= <opt attributes> <opt modifiers> enum <type declaration name> <opt enum base> '{' <opt enum member declarations> '}' <opt semicolon>")]
			public EnumDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers, Semantic _symbol92,TypeDeclarationName _TypeDeclarationName,OptEnumBase _OptEnumBase, Semantic _symbol43,OptEnumMemberDeclarations _OptEnumMemberDeclarations, Semantic _symbol47,OptSemicolon _OptSemicolon)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_type_declaration_name = _TypeDeclarationName;
				_opt_enum_base = _OptEnumBase;
				_opt_enum_member_declarations = _OptEnumMemberDeclarations;
				_opt_semicolon = _OptSemicolon;
				}
}
}
