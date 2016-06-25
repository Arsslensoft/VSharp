using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptArrayInitializer : Semantic {
 			public ArrayInitializer _array_initializer;

			[Rule("<opt array initializer> ::= ")]
			public OptArrayInitializer()
				{
				}
			[Rule("<opt array initializer> ::= <array initializer>")]
			public OptArrayInitializer(ArrayInitializer _ArrayInitializer)
				{
				_array_initializer = _ArrayInitializer;
				}
}
}
