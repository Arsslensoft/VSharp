using System;
using System.Collections.Generic;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
namespace VSC.AST { 
	public class AttributeSection : ResolvableSemantic {
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
