using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ConstVariableInitializer : Semantic {
 			public VariableInitializer _variable_initializer;

			[Rule("<const variable initializer> ::= ")]
			public ConstVariableInitializer()
				{
				}
			[Rule("<const variable initializer> ::= '=' <variable initializer>")]
			public ConstVariableInitializer( Semantic _symbol60,VariableInitializer _VariableInitializer)
				{
				_variable_initializer = _VariableInitializer;
				}
}
}
