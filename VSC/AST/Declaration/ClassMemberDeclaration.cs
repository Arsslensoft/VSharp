using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ClassMemberDeclaration : Semantic {
 			public ConstantDeclaration _constant_declaration;
			public FieldDeclaration _field_declaration;
			public MethodDeclaration _method_declaration;
			public PropertyDeclaration _property_declaration;
			public EventDeclaration _event_declaration;
			public IndexerDeclaration _indexer_declaration;
			public OperatorDeclaration _operator_declaration;
			public ConstructorDeclaration _constructor_declaration;
			public DestructorDeclaration _destructor_declaration;

			[Rule("<class member declaration> ::= <constant declaration>")]
			public ClassMemberDeclaration(ConstantDeclaration _ConstantDeclaration)
				{
				_constant_declaration = _ConstantDeclaration;
				}
			[Rule("<class member declaration> ::= <field declaration>")]
			public ClassMemberDeclaration(FieldDeclaration _FieldDeclaration)
				{
				_field_declaration = _FieldDeclaration;
				}
			[Rule("<class member declaration> ::= <method declaration>")]
			public ClassMemberDeclaration(MethodDeclaration _MethodDeclaration)
				{
				_method_declaration = _MethodDeclaration;
				}
			[Rule("<class member declaration> ::= <property declaration>")]
			public ClassMemberDeclaration(PropertyDeclaration _PropertyDeclaration)
				{
				_property_declaration = _PropertyDeclaration;
				}
			[Rule("<class member declaration> ::= <event declaration>")]
			public ClassMemberDeclaration(EventDeclaration _EventDeclaration)
				{
				_event_declaration = _EventDeclaration;
				}
			[Rule("<class member declaration> ::= <indexer declaration>")]
			public ClassMemberDeclaration(IndexerDeclaration _IndexerDeclaration)
				{
				_indexer_declaration = _IndexerDeclaration;
				}
			[Rule("<class member declaration> ::= <operator declaration>")]
			public ClassMemberDeclaration(OperatorDeclaration _OperatorDeclaration)
				{
				_operator_declaration = _OperatorDeclaration;
				}
			[Rule("<class member declaration> ::= <constructor declaration>")]
			public ClassMemberDeclaration(ConstructorDeclaration _ConstructorDeclaration)
				{
				_constructor_declaration = _ConstructorDeclaration;
				}
			[Rule("<class member declaration> ::= <destructor declaration>")]
			public ClassMemberDeclaration(DestructorDeclaration _DestructorDeclaration)
				{
				_destructor_declaration = _DestructorDeclaration;
				}
}
}
