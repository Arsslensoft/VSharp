using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptEventInitializer : Semantic {
 			public EventVariableInitializer _event_variable_initializer;

			[Rule("<opt event initializer> ::= ")]
			public OptEventInitializer()
				{
				}
			[Rule("<opt event initializer> ::= '=' <event variable initializer>")]
			public OptEventInitializer( Semantic _symbol60,EventVariableInitializer _EventVariableInitializer)
				{
				_event_variable_initializer = _EventVariableInitializer;
				}
}
}
