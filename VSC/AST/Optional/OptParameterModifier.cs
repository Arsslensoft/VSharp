using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptParameterModifier : Semantic {
 			public ParameterModifier _parameter_modifier;

			[Rule("<opt parameter modifier> ::= ")]
			public OptParameterModifier()
				{
				}
			[Rule("<opt parameter modifier> ::= <parameter modifier>")]
			public OptParameterModifier(ParameterModifier _ParameterModifier)
				{
				_parameter_modifier = _ParameterModifier;
				}
}
}
