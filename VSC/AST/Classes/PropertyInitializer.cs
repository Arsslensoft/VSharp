using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class PropertyInitializer : Semantic {
 			public Expression _expression;
			public ArrayInitializer _array_initializer;

			[Rule("<property initializer> ::= <expression>")]
			public PropertyInitializer(Expression _Expression)
				{
				_expression = _Expression;
				}
			[Rule("<property initializer> ::= <array initializer>")]
			public PropertyInitializer(ArrayInitializer _ArrayInitializer)
				{
				_array_initializer = _ArrayInitializer;
				}
}
}
