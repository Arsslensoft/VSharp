using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class InterruptStatement : Statement
    {
 			public IntegralLiteral _integral_constant;
			public ExpressionList _expression_list;

			[Rule("<interrupt statement> ::= interrupt <Integral Constant> ';'")]
			public InterruptStatement( Semantic _symbol112,IntegralLiteral _IntegralConstant, Semantic _symbol31)
				{
				_integral_constant = _IntegralConstant;
				}
			[Rule("<interrupt statement> ::= interrupt <Integral Constant> '(' <expression list> ')' ';'")]
			public InterruptStatement( Semantic _symbol112,IntegralLiteral _IntegralConstant, Semantic _symbol20,ExpressionList _ExpressionList, Semantic _symbol21, Semantic _symbol31)
				{
				_integral_constant = _IntegralConstant;
				_expression_list = _ExpressionList;
				}
}
}
