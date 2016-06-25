using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class EventDeclarator : Semantic {
 			public Identifier _identifier;
			public EventVariableInitializer _event_variable_initializer;

			[Rule("<event declarator> ::= ',' <Identifier>")]
			public EventDeclarator( Semantic _symbol24,Identifier _Identifier)
				{
				_identifier = _Identifier;
				}
			[Rule("<event declarator> ::= ',' <Identifier> '=' <event variable initializer>")]
			public EventDeclarator( Semantic _symbol24,Identifier _Identifier, Semantic _symbol60,EventVariableInitializer _EventVariableInitializer)
				{
				_identifier = _Identifier;
				_event_variable_initializer = _EventVariableInitializer;
				}
}
}
