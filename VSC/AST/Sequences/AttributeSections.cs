using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class AttributeSections : Sequence<AttributeSection> {
			[Rule("<attribute sections> ::= <attribute section>")]
			public AttributeSections(AttributeSection _AttributeSection) : base(_AttributeSection)
				{
			
				}
			[Rule("<attribute sections> ::= <attribute sections> <attribute section>")]
			public AttributeSections(AttributeSections _AttributeSections,AttributeSection _AttributeSection) : base(_AttributeSection,_AttributeSections)
				{
				}
}
}
