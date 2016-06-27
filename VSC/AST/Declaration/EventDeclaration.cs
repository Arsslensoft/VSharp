using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class EventDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public Type _type;
			public MemberDeclarationName _member_declaration_name;
			public OptEventInitializer _opt_event_initializer;
			public OptEventDeclarators _opt_event_declarators;
			public EventAccessorDeclarations _event_accessor_declarations;

			[Rule("<event declaration> ::= <opt attributes> <opt modifiers> event <type> <member declaration name> <opt event initializer> <opt event declarators> ';'")]
			public EventDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers, Semantic _symbol93,Type _Type,MemberDeclarationName _MemberDeclarationName,OptEventInitializer _OptEventInitializer,OptEventDeclarators _OptEventDeclarators, Semantic _symbol31)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_type = _Type;
				_member_declaration_name = _MemberDeclarationName;
				_opt_event_initializer = _OptEventInitializer;
				_opt_event_declarators = _OptEventDeclarators;
				}
			[Rule("<event declaration> ::= <opt attributes> <opt modifiers> event <type> <member declaration name> '{' <event accessor declarations> '}'")]
			public EventDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers, Semantic _symbol93,Type _Type,MemberDeclarationName _MemberDeclarationName, Semantic _symbol43,EventAccessorDeclarations _EventAccessorDeclarations, Semantic _symbol47)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_type = _Type;
				_member_declaration_name = _MemberDeclarationName;
				_event_accessor_declarations = _EventAccessorDeclarations;
				}
}
}
