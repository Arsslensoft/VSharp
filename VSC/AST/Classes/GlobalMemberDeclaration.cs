using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class GlobalMemberDeclaration : Semantic {
 			public ConstantDeclaration _constant_declaration;
			public FieldDeclaration _field_declaration;
			public MethodDeclaration _method_declaration;
			public PropertyDeclaration _property_declaration;
			public EventDeclaration _event_declaration;
			public IndexerDeclaration _indexer_declaration;
			public OperatorDeclaration _operator_declaration;
			public InterruptDeclaration _interrupt_declaration;

			[Rule("<global member declaration> ::= <constant declaration>")]
			public GlobalMemberDeclaration(ConstantDeclaration _ConstantDeclaration)
				{
				_constant_declaration = _ConstantDeclaration;
				}
			[Rule("<global member declaration> ::= <field declaration>")]
			public GlobalMemberDeclaration(FieldDeclaration _FieldDeclaration)
				{
				_field_declaration = _FieldDeclaration;
				}
			[Rule("<global member declaration> ::= <method declaration>")]
			public GlobalMemberDeclaration(MethodDeclaration _MethodDeclaration)
				{
				_method_declaration = _MethodDeclaration;
				}
			[Rule("<global member declaration> ::= <property declaration>")]
			public GlobalMemberDeclaration(PropertyDeclaration _PropertyDeclaration)
				{
				_property_declaration = _PropertyDeclaration;
				}
			[Rule("<global member declaration> ::= <event declaration>")]
			public GlobalMemberDeclaration(EventDeclaration _EventDeclaration)
				{
				_event_declaration = _EventDeclaration;
				}
			[Rule("<global member declaration> ::= <indexer declaration>")]
			public GlobalMemberDeclaration(IndexerDeclaration _IndexerDeclaration)
				{
				_indexer_declaration = _IndexerDeclaration;
				}
			[Rule("<global member declaration> ::= <operator declaration>")]
			public GlobalMemberDeclaration(OperatorDeclaration _OperatorDeclaration)
				{
				_operator_declaration = _OperatorDeclaration;
				}
			[Rule("<global member declaration> ::= <interrupt declaration>")]
			public GlobalMemberDeclaration(InterruptDeclaration _InterruptDeclaration)
				{
				_interrupt_declaration = _InterruptDeclaration;
				}
}
}
