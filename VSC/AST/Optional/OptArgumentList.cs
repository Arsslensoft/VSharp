using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptArgumentList : Semantic {
 			public ArgumentList _argument_list;

			[Rule("<opt argument list> ::= ")]
			public OptArgumentList()
				{
				}
			[Rule("<opt argument list> ::= <argument list>")]
			public OptArgumentList(ArgumentList _ArgumentList)
				{
				_argument_list = _ArgumentList;
				}
}
}
