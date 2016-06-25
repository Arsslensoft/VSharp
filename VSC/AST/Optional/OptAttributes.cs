using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptAttributes : Semantic {
 			public AttributeSections _attribute_sections;

			[Rule("<opt attributes> ::= ")]
			public OptAttributes()
				{
				}
			[Rule("<opt attributes> ::= <attribute sections>")]
			public OptAttributes(AttributeSections _AttributeSections)
				{
				_attribute_sections = _AttributeSections;
				}
}
}
