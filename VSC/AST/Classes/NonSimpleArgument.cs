using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class NonSimpleArgument : Semantic {
 			public Expression _expression;

			[Rule("<non simple argument> ::= ref <expression>")]
			[Rule("<non simple argument> ::= out <expression>")]
			public NonSimpleArgument( Semantic _symbol132,Expression _Expression)
				{
				_expression = _Expression;
				}
}
}
