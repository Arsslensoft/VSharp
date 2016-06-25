using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptPropertyInitializer : Semantic {
 			public PropertyInitializer _property_initializer;

			[Rule("<opt property initializer> ::= ")]
			public OptPropertyInitializer()
				{
				}
			[Rule("<opt property initializer> ::= '=' <property initializer> ';'")]
			public OptPropertyInitializer( Semantic _symbol60,PropertyInitializer _PropertyInitializer, Semantic _symbol31)
				{
				_property_initializer = _PropertyInitializer;
				}
}
}
