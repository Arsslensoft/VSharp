using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class DimSeparators : Sequence<Semantic> {

			[Rule("<dim separators> ::= ','")]
			public DimSeparators( Semantic _symbol24) :base(_symbol24)
				{
				}
			[Rule("<dim separators> ::= <dim separators> ','")]
			public DimSeparators(DimSeparators _DimSeparators, Semantic _symbol24) : base(_symbol24,_DimSeparators)
				{
	
				}
}
}
