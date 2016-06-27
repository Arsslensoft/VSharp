using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class IntegralType : Semantic {

        internal string _Keyword;
			[Rule("<integral type> ::= sbyte")]
			[Rule("<integral type> ::= byte")]
			[Rule("<integral type> ::= short")]
			[Rule("<integral type> ::= ushort")]
			[Rule("<integral type> ::= int")]
			[Rule("<integral type> ::= uint")]
			[Rule("<integral type> ::= long")]
			[Rule("<integral type> ::= ulong")]
			[Rule("<integral type> ::= char")]
			public IntegralType( Semantic _symbol136)
				{
                    _Keyword = _symbol136.Name;
				}
}
}
