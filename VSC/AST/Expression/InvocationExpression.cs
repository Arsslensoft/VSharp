using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class InvocationExpression : Expression
    {
 			public Expression _primary_expression;
			public OptArgumentList _opt_argument_list;

			[Rule("<invocation expression> ::= <primary expression> '(' <opt argument list> ')'")]
			public InvocationExpression(Expression _PrimaryExpression, Semantic _symbol20,OptArgumentList _OptArgumentList, Semantic _symbol21)
				{
				_primary_expression = _PrimaryExpression;
				_opt_argument_list = _OptArgumentList;
				}
}
}
