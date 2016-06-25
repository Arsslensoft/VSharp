using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class RankSpecifier : Semantic {
 			public DimSeparators _dim_separators;

			[Rule("<rank specifier> ::= '[]'")]
			public RankSpecifier( Semantic _symbol39)
				{
				}
			[Rule("<rank specifier> ::= '[,' <dim separators> ']'")]
			public RankSpecifier( Semantic _symbol38,DimSeparators _DimSeparators, Semantic _symbol40)
				{
				_dim_separators = _DimSeparators;
				}
}
}
