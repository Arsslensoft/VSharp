using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptConstructorInitializer : Semantic {
 			public ConstructorInitializer _constructor_initializer;

			[Rule("<opt constructor initializer> ::= ")]
			public OptConstructorInitializer()
				{
				}
			[Rule("<opt constructor initializer> ::= <constructor initializer>")]
			public OptConstructorInitializer(ConstructorInitializer _ConstructorInitializer)
				{
				_constructor_initializer = _ConstructorInitializer;
				}
}
}
