using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptFieldInitializer : Semantic {
 			public VariableInitializer _variable_initializer;

			[Rule("<opt field initializer> ::= ")]
			public OptFieldInitializer()
				{
				}
			[Rule("<opt field initializer> ::= '=' <variable initializer>")]
			public OptFieldInitializer( Semantic _symbol60,VariableInitializer _VariableInitializer)
				{
				_variable_initializer = _VariableInitializer;
				}
}
}
