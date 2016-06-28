using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptAttributeTarget : Semantic {
        public bool _IsReturn = false;
			[Rule("<opt attribute target> ::= return ':'")]
			public OptAttributeTarget( Semantic _symbol135, Semantic _symbol28)
				{
                    _IsReturn = true;
				}
			[Rule("<opt attribute target> ::= ")]
			public OptAttributeTarget()
				{
				}
}
}
