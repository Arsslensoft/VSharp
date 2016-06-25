using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class VariableInitializer : Semantic {
 			public Expression _expression;
			public ArrayInitializer _array_initializer;

			[Rule("<variable initializer> ::= <expression>")]
			public VariableInitializer(Expression _Expression)
				{
				_expression = _Expression;
				}
			[Rule("<variable initializer> ::= <array initializer>")]
			public VariableInitializer(ArrayInitializer _ArrayInitializer)
				{
				_array_initializer = _ArrayInitializer;
				}
}
}
