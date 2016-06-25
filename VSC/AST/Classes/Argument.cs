using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class Argument : Semantic {
 			public Expression _expression;
			public NonSimpleArgument _non_simple_argument;

			[Rule("<argument> ::= <expression>")]
			public Argument(Expression _Expression)
				{
				_expression = _Expression;
				}
			[Rule("<argument> ::= <non simple argument>")]
			public Argument(NonSimpleArgument _NonSimpleArgument)
				{
				_non_simple_argument = _NonSimpleArgument;
				}
}
}
