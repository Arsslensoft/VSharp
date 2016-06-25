using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ArrayInitializer : Semantic {
 			public VariableInitializerList _variable_initializer_list;
			public OptComma _opt_comma;

			[Rule("<array initializer> ::= '{' '}'")]
			public ArrayInitializer( Semantic _symbol43, Semantic _symbol47)
				{
				}
			[Rule("<array initializer> ::= '{' <variable initializer list> <opt comma> '}'")]
			public ArrayInitializer( Semantic _symbol43,VariableInitializerList _VariableInitializerList,OptComma _OptComma, Semantic _symbol47)
				{
				_variable_initializer_list = _VariableInitializerList;
				_opt_comma = _OptComma;
				}
}
}
