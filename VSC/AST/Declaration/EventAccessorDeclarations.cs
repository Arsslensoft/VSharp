using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class EventAccessorDeclarations : Semantic {
 			public AddAccessorDeclaration _add_accessor_declaration;
			public RemoveAccessorDeclaration _remove_accessor_declaration;
			public RaiseAccessorDeclarationOpt _raise_accessor_declaration_opt;


			[Rule("<event accessor declarations> ::= <add accessor declaration> <remove accessor declaration> <raise accessor declaration opt>")]
			public EventAccessorDeclarations(AddAccessorDeclaration _AddAccessorDeclaration,RemoveAccessorDeclaration _RemoveAccessorDeclaration,RaiseAccessorDeclarationOpt _RaiseAccessorDeclarationOpt)
				{
				_add_accessor_declaration = _AddAccessorDeclaration;
				_remove_accessor_declaration = _RemoveAccessorDeclaration;
				_raise_accessor_declaration_opt = _RaiseAccessorDeclarationOpt;
				}
			[Rule("<event accessor declarations> ::= <remove accessor declaration> <add accessor declaration> <raise accessor declaration opt>")]
			public EventAccessorDeclarations(RemoveAccessorDeclaration _RemoveAccessorDeclaration,AddAccessorDeclaration _AddAccessorDeclaration,RaiseAccessorDeclarationOpt _RaiseAccessorDeclarationOpt)
				{
				_remove_accessor_declaration = _RemoveAccessorDeclaration;
				_add_accessor_declaration = _AddAccessorDeclaration;
				_raise_accessor_declaration_opt = _RaiseAccessorDeclarationOpt;
				}
			[Rule("<event accessor declarations> ::= <add accessor declaration> <raise accessor declaration opt>")]
			public EventAccessorDeclarations(AddAccessorDeclaration _AddAccessorDeclaration,RaiseAccessorDeclarationOpt _RaiseAccessorDeclarationOpt)
				{
				_add_accessor_declaration = _AddAccessorDeclaration;
				_raise_accessor_declaration_opt = _RaiseAccessorDeclarationOpt;
				}
			[Rule("<event accessor declarations> ::= <remove accessor declaration> <raise accessor declaration opt>")]
			public EventAccessorDeclarations(RemoveAccessorDeclaration _RemoveAccessorDeclaration,RaiseAccessorDeclarationOpt _RaiseAccessorDeclarationOpt)
				{
				_remove_accessor_declaration = _RemoveAccessorDeclaration;
				_raise_accessor_declaration_opt = _RaiseAccessorDeclarationOpt;
				}
	
}
}
