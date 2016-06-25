using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptLambdaParameterList : Semantic {
 			public LambdaParameterList _lambda_parameter_list;

			[Rule("<opt lambda parameter list> ::= ")]
			public OptLambdaParameterList()
				{
				}
			[Rule("<opt lambda parameter list> ::= <lambda parameter list>")]
			public OptLambdaParameterList(LambdaParameterList _LambdaParameterList)
				{
				_lambda_parameter_list = _LambdaParameterList;
				}
}
}
