using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ClassBase : Semantic {
 			public TypeList _type_list;

			[Rule("<class base> ::= ':' <type list>")]
			public ClassBase( Semantic _symbol28,TypeList _TypeList)
				{
				_type_list = _TypeList;
				}
}
}
