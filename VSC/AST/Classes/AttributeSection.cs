using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class AttributeSection : Semantic {
 			public OptAttributeTarget _opt_attribute_target;
			public AttributeList _attribute_list;

			[Rule("<attribute section> ::= '[' <opt attribute target> <attribute list> ']'")]
			public AttributeSection( Semantic _symbol37,OptAttributeTarget _OptAttributeTarget,AttributeList _AttributeList, Semantic _symbol40)
				{
				_opt_attribute_target = _OptAttributeTarget;
				_attribute_list = _AttributeList;
				}
			[Rule("<attribute section> ::= '[' <opt attribute target> <attribute list> ',' ']'")]
			public AttributeSection( Semantic _symbol37,OptAttributeTarget _OptAttributeTarget,AttributeList _AttributeList, Semantic _symbol24, Semantic _symbol40)
				{
				_opt_attribute_target = _OptAttributeTarget;
				_attribute_list = _AttributeList;
				}
}
}
