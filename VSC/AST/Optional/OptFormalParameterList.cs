using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptFormalParameterList : Semantic {
 			public FormalParameterList _formal_parameter_list;

			[Rule("<opt formal parameter list> ::= ")]
			public OptFormalParameterList()
				{
				}
			[Rule("<opt formal parameter list> ::= <formal parameter list>")]
			public OptFormalParameterList(FormalParameterList _FormalParameterList)
				{
				_formal_parameter_list = _FormalParameterList;
				}
}
}
