using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class AttributeList : Sequence<Attribute> {
 		
			[Rule("<attribute list> ::= <attribute>")]
			public AttributeList(Attribute _Attribute) :base(_Attribute)
				{
		
				}
			[Rule("<attribute list> ::= <attribute list> ',' <attribute>")]
			public AttributeList(AttributeList _AttributeList, Semantic _symbol24,Attribute _Attribute) : base(_Attribute,_AttributeList)
				{
		
				}
}
}
