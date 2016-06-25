using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class PropertyDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public MemberType _member_type;
			public MemberDeclarationName _member_declaration_name;
			public AccessorDeclarations _accessor_declarations;
			public OptPropertyInitializer _opt_property_initializer;
			public ExpressionBlock _expression_block;

			[Rule("<property declaration> ::= <opt attributes> <opt modifiers> <member type> <member declaration name> '{' <accessor declarations> '}' <opt property initializer>")]
			public PropertyDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers,MemberType _MemberType,MemberDeclarationName _MemberDeclarationName, Semantic _symbol43,AccessorDeclarations _AccessorDeclarations, Semantic _symbol47,OptPropertyInitializer _OptPropertyInitializer)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_member_type = _MemberType;
				_member_declaration_name = _MemberDeclarationName;
				_accessor_declarations = _AccessorDeclarations;
				_opt_property_initializer = _OptPropertyInitializer;
				}
			[Rule("<property declaration> ::= <opt attributes> <opt modifiers> <member type> <member declaration name> <expression block>")]
			public PropertyDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers,MemberType _MemberType,MemberDeclarationName _MemberDeclarationName,ExpressionBlock _ExpressionBlock)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_member_type = _MemberType;
				_member_declaration_name = _MemberDeclarationName;
				_expression_block = _ExpressionBlock;
				}
}
}
