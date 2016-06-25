using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptEventDeclarators : Semantic {
 			public EventDeclarators _event_declarators;

			[Rule("<opt event declarators> ::= ")]
			public OptEventDeclarators()
				{
				}
			[Rule("<opt event declarators> ::= <event declarators>")]
			public OptEventDeclarators(EventDeclarators _EventDeclarators)
				{
				_event_declarators = _EventDeclarators;
				}
}
}
