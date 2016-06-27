using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class Identifier : Semantic {

        internal string _Identifier;
			[Rule("<Identifier> ::= Identifier")]
			public Identifier( Semantic _symbol104)
				{
                    _Identifier = _symbol104.Name;
				}
}
}
