using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class EqualityExpression : Expression
    {
 			public RelationalExpression _relational_expression;
			public EqualityExpression _equality_expression;

			[Rule("<equality expression> ::= <relational expression>")]
			public EqualityExpression(RelationalExpression _RelationalExpression)
				{
				_relational_expression = _RelationalExpression;
				}
			[Rule("<equality expression> ::= <equality expression> '==' <relational expression>")]
			[Rule("<equality expression> ::= <equality expression> '!=' <relational expression>")]
			public EqualityExpression(EqualityExpression _EqualityExpression, Semantic _symbol62,RelationalExpression _RelationalExpression)
				{
				_equality_expression = _EqualityExpression;
				_relational_expression = _RelationalExpression;
				}
}
}
