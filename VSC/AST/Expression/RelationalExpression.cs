using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class RelationalExpression : Expression
    {
 			public ShiftExpression _shift_expression;
			public RelationalExpression _relational_expression;

			[Rule("<relational expression> ::= <shift expression>")]
			public RelationalExpression(ShiftExpression _ShiftExpression)
				{
				_shift_expression = _ShiftExpression;
				}
			[Rule("<relational expression> ::= <relational expression> '<' <shift expression>")]
			[Rule("<relational expression> ::= <relational expression> '>' <shift expression>")]
			[Rule("<relational expression> ::= <relational expression> '<=' <shift expression>")]
			[Rule("<relational expression> ::= <relational expression> '>=' <shift expression>")]
			public RelationalExpression(RelationalExpression _RelationalExpression, Semantic _symbol54,ShiftExpression _ShiftExpression)
				{
				_relational_expression = _RelationalExpression;
				_shift_expression = _ShiftExpression;
				}
}
}
