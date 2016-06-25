using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ReturnStatement : Statement
    {
 			public ExpressionOpt _expression_opt;

			[Rule("<return statement> ::= return <Expression Opt> ';'")]
			public ReturnStatement( Semantic _symbol135,ExpressionOpt _ExpressionOpt, Semantic _symbol31)
				{
				_expression_opt = _ExpressionOpt;
				}
}
}
