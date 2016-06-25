using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class LambdaExpression : Expression
    {
 			public Identifier _identifier;
			public LambdaExpressionBody _lambda_expression_body;
			public OptLambdaParameterList _opt_lambda_parameter_list;

			[Rule("<lambda expression> ::= <Identifier> '=>' <lambda expression body>")]
			public LambdaExpression(Identifier _Identifier, Semantic _symbol63,LambdaExpressionBody _LambdaExpressionBody)
				{
				_identifier = _Identifier;
				_lambda_expression_body = _LambdaExpressionBody;
				}
			[Rule("<lambda expression> ::= '(' <opt lambda parameter list> ')' '=>' <lambda expression body>")]
			public LambdaExpression( Semantic _symbol20,OptLambdaParameterList _OptLambdaParameterList, Semantic _symbol21, Semantic _symbol63,LambdaExpressionBody _LambdaExpressionBody)
				{
				_opt_lambda_parameter_list = _OptLambdaParameterList;
				_lambda_expression_body = _LambdaExpressionBody;
				}
}
}
