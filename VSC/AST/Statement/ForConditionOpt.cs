using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ForConditionOpt : Semantic
    {
 			public Expression _expression;

			[Rule("<For Condition Opt> ::= <expression>")]
			public ForConditionOpt(Expression _Expression)
				{
				_expression = _Expression;
				}
			[Rule("<For Condition Opt> ::= ")]
			public ForConditionOpt()
				{
				}
}
}
