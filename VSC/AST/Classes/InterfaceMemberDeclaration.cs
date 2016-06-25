using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class InterfaceMemberDeclaration : Semantic {
 			public ConstantDeclaration _constant_declaration;
			public FieldDeclaration _field_declaration;
			public MethodDeclaration _method_declaration;
			public PropertyDeclaration _property_declaration;
			public EventDeclaration _event_declaration;
			public IndexerDeclaration _indexer_declaration;
			public OperatorDeclaration _operator_declaration;
			public ConstructorDeclaration _constructor_declaration;
			public TypeDeclaration _type_declaration;

			[Rule("<interface member declaration> ::= <constant declaration>")]
			public InterfaceMemberDeclaration(ConstantDeclaration _ConstantDeclaration)
				{
				_constant_declaration = _ConstantDeclaration;
				}
			[Rule("<interface member declaration> ::= <field declaration>")]
			public InterfaceMemberDeclaration(FieldDeclaration _FieldDeclaration)
				{
				_field_declaration = _FieldDeclaration;
				}
			[Rule("<interface member declaration> ::= <method declaration>")]
			public InterfaceMemberDeclaration(MethodDeclaration _MethodDeclaration)
				{
				_method_declaration = _MethodDeclaration;
				}
			[Rule("<interface member declaration> ::= <property declaration>")]
			public InterfaceMemberDeclaration(PropertyDeclaration _PropertyDeclaration)
				{
				_property_declaration = _PropertyDeclaration;
				}
			[Rule("<interface member declaration> ::= <event declaration>")]
			public InterfaceMemberDeclaration(EventDeclaration _EventDeclaration)
				{
				_event_declaration = _EventDeclaration;
				}
			[Rule("<interface member declaration> ::= <indexer declaration>")]
			public InterfaceMemberDeclaration(IndexerDeclaration _IndexerDeclaration)
				{
				_indexer_declaration = _IndexerDeclaration;
				}
			[Rule("<interface member declaration> ::= <operator declaration>")]
			public InterfaceMemberDeclaration(OperatorDeclaration _OperatorDeclaration)
				{
				_operator_declaration = _OperatorDeclaration;
				}
			[Rule("<interface member declaration> ::= <constructor declaration>")]
			public InterfaceMemberDeclaration(ConstructorDeclaration _ConstructorDeclaration)
				{
				_constructor_declaration = _ConstructorDeclaration;
				}
			[Rule("<interface member declaration> ::= <type declaration>")]
			public InterfaceMemberDeclaration(TypeDeclaration _TypeDeclaration)
				{
				_type_declaration = _TypeDeclaration;
				}
}
}
