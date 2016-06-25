using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ArgumentList : Sequence<Argument> {
 		
			[Rule("<argument list> ::= <argument>")]
			public ArgumentList(Argument _Argument) : base(_Argument)
				{
				
				}
			[Rule("<argument list> ::= <argument list> ',' <argument>")]
			public ArgumentList(ArgumentList _ArgumentList, Semantic _symbol24,Argument _Argument) : base(_Argument, _ArgumentList)
				{
				}
}
}
