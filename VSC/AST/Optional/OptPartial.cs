using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptPartial : Semantic {

        public bool ispartial = false;
			[Rule("<opt partial> ::= partial")]
			public OptPartial( Semantic _symbol24)
        {
            ispartial = true;
				}
            [Rule("<opt partial> ::= ")]
            public OptPartial()
				{
				}
}
}
