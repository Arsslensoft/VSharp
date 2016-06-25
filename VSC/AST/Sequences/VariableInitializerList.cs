using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class VariableInitializerList : Sequence<VariableInitializer> {

			[Rule("<variable initializer list> ::= <variable initializer>")]
			public VariableInitializerList(VariableInitializer _VariableInitializer) : base(_VariableInitializer)
				{
				
				}
			[Rule("<variable initializer list> ::= <variable initializer list> ',' <variable initializer>")]
			public VariableInitializerList(VariableInitializerList _VariableInitializerList, Semantic _symbol24,VariableInitializer _VariableInitializer) : base(_VariableInitializer,_VariableInitializerList)
				{
				
				
				}
}
}
