using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptAttributeTarget : Semantic {
 
			[Rule("<opt attribute target> ::= return ':'")]
			public OptAttributeTarget( Semantic _symbol135, Semantic _symbol28)
				{
				}
			[Rule("<opt attribute target> ::= ")]
			public OptAttributeTarget()
				{
				}
}
}
