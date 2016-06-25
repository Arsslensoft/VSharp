using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class EventDeclarators : Sequence<EventDeclarator> {

			[Rule("<event declarators> ::= <event declarator>")]
			public EventDeclarators(EventDeclarator _EventDeclarator) : base(_EventDeclarator)
				{
				
				}
			[Rule("<event declarators> ::= <event declarators> <event declarator>")]
			public EventDeclarators(EventDeclarators _EventDeclarators,EventDeclarator _EventDeclarator) : base(_EventDeclarator,_EventDeclarators)
				{
				}
}
}
