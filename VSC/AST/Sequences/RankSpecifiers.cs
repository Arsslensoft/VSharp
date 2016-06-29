using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class RankSpecifiers : Sequence<RankSpecifier> {
			[Rule("<rank specifiers> ::= <rank specifier>")]
			public RankSpecifiers(RankSpecifier _RankSpecifier) : base(_RankSpecifier)
				{
				
				}
			[Rule("<rank specifiers> ::= <rank specifier> <rank specifiers>")]
			public RankSpecifiers(RankSpecifier _RankSpecifier,RankSpecifiers _RankSpecifiers) : base(_RankSpecifier,_RankSpecifiers)
				{
				}
            public override string ToString()
            {
                string res = "";
                foreach (var rs in this)
                    res += rs.ToString();
                return res;
            }
}
}
