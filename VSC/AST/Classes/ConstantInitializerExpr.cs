using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ConstantInitializerExpr : Semantic {
 			public Expression _expression;
			public ArrayInitializer _array_initializer;

			[Rule("<constant initializer expr> ::= <expression>")]
			public ConstantInitializerExpr(Expression _Expression)
				{
				_expression = _Expression;
				}
			[Rule("<constant initializer expr> ::= <array initializer>")]
			public ConstantInitializerExpr(ArrayInitializer _ArrayInitializer)
				{
				_array_initializer = _ArrayInitializer;
				}
}
}
