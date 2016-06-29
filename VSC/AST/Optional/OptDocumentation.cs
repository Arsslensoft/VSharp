using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptDocumentation : Semantic {

        public Documentation _documentation;
			[Rule("<Opt Documentation> ::= <Documentation>")]
			public OptDocumentation( Documentation _symbol24)
				{
                    _documentation = _symbol24;
				}
            [Rule("<Opt Documentation> ::= ")]
            public OptDocumentation()
				{
				}
}
}
