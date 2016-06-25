using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class FormalParameterList : Semantic {
 			public FixedParameters _fixed_parameters;
			public ParameterArray _parameter_array;

			[Rule("<formal parameter list> ::= <fixed parameters>")]
			public FormalParameterList(FixedParameters _FixedParameters)
				{
				_fixed_parameters = _FixedParameters;
				}
			[Rule("<formal parameter list> ::= <fixed parameters> ',' <parameter array>")]
			public FormalParameterList(FixedParameters _FixedParameters, Semantic _symbol24,ParameterArray _ParameterArray)
				{
				_fixed_parameters = _FixedParameters;
				_parameter_array = _ParameterArray;
				}
}
}
