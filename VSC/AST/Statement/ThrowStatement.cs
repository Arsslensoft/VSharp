using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ThrowStatement : Statement
    {
 			public ExpressionOpt _expression_opt;

			[Rule("<Throw Statement> ::= throw <Expression Opt> ';'")]
			public ThrowStatement( Semantic _symbol149,ExpressionOpt _ExpressionOpt, Semantic _symbol31)
				{
				_expression_opt = _ExpressionOpt;
				}
}
}
