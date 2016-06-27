using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptTypeArgumentList : Semantic {
 			public TypeArguments _type_arguments;

   
			[Rule("<opt type argument list> ::= ")]
			public OptTypeArgumentList()
				{
				}
			[Rule("<opt type argument list> ::= '!<' <type arguments> '>'")]
			public OptTypeArgumentList( Semantic _symbol11,TypeArguments _TypeArguments, Semantic _symbol64)
				{
				_type_arguments = _TypeArguments;
				}
}
}
