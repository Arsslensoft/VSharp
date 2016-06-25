using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptLocalVariableInitializer : Semantic {
 			public BlockVariableInitializer _block_variable_initializer;

			[Rule("<opt local variable initializer> ::= ")]
			public OptLocalVariableInitializer()
				{
				}
			[Rule("<opt local variable initializer> ::= '=' <block variable initializer>")]
			public OptLocalVariableInitializer( Semantic _symbol60,BlockVariableInitializer _BlockVariableInitializer)
				{
				_block_variable_initializer = _BlockVariableInitializer;
				}
}
}
