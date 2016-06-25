using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class LambdaParameterList : Sequence<LambdaParameter> {
 			
			

			[Rule("<lambda parameter list> ::= <lambda parameter>")]
			public LambdaParameterList(LambdaParameter _LambdaParameter) : base(_LambdaParameter)
				{
			
				}
			[Rule("<lambda parameter list> ::= <lambda parameter list> ',' <lambda parameter>")]
			public LambdaParameterList(LambdaParameterList _LambdaParameterList, Semantic _symbol24,LambdaParameter _LambdaParameter) : base(_LambdaParameter,_LambdaParameterList)
				{
				}
}
}
