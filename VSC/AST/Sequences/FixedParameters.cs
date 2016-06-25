using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class FixedParameters : Sequence<FixedParameter> {
 			

			[Rule("<fixed parameters> ::= <fixed parameter>")]
			public FixedParameters(FixedParameter _FixedParameter) : base(_FixedParameter)
				{
				
				}
			[Rule("<fixed parameters> ::= <fixed parameters> ',' <fixed parameter>")]
			public FixedParameters(FixedParameters _FixedParameters, Semantic _symbol24,FixedParameter _FixedParameter) : base(_FixedParameter,_FixedParameters)
				{
				
				
				}
}
}
