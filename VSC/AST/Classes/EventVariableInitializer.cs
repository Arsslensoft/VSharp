using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class EventVariableInitializer : Semantic {
 			public VariableInitializer _variable_initializer;

			[Rule("<event variable initializer> ::= <variable initializer>")]
			public EventVariableInitializer(VariableInitializer _VariableInitializer)
				{
				_variable_initializer = _VariableInitializer;
				}
}
}
