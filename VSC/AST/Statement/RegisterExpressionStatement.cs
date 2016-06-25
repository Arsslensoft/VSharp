using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class RegisterExpressionStatement : Statement
    {
 			public RegisterExpression _register_expression;

			[Rule("<register expression statement> ::= <Register Expression> ';'")]
			public RegisterExpressionStatement(RegisterExpression _RegisterExpression, Semantic _symbol31)
				{
				_register_expression = _RegisterExpression;
				}
}
}
