using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptEnumBase : Semantic {
 			public Type _type;

			[Rule("<opt enum base> ::= ")]
			public OptEnumBase()
				{
				}
			[Rule("<opt enum base> ::= ':' <type>")]
			public OptEnumBase( Semantic _symbol28,Type _Type)
				{
				_type = _Type;
				}
}
}
