using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ExpressionOpt : Semantic {
 			public Expression _expression;

			[Rule("<Expression Opt> ::= <expression>")]
			public ExpressionOpt(Expression _Expression)
				{
				_expression = _Expression;
				}
			[Rule("<Expression Opt> ::= ")]
			public ExpressionOpt()
				{
				}
}
}
