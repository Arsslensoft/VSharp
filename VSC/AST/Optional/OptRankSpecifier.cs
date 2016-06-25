using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptRankSpecifier : Semantic {
 			public RankSpecifiers _rank_specifiers;

			[Rule("<opt rank specifier> ::= ")]
			public OptRankSpecifier()
				{
				}
			[Rule("<opt rank specifier> ::= <rank specifiers>")]
			public OptRankSpecifier(RankSpecifiers _RankSpecifiers)
				{
				_rank_specifiers = _RankSpecifiers;
				}
}
}
