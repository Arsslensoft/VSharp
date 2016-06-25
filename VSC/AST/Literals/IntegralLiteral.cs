using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class IntegralLiteral : Semantic {
 
			[Rule("<Integral Constant> ::= OctalLiteral")]
			[Rule("<Integral Constant> ::= HexLiteral")]
			[Rule("<Integral Constant> ::= DecLiteral")]
			[Rule("<Integral Constant> ::= BinaryLiteral")]
			public IntegralLiteral( Semantic _symbol118)
				{
				}
}
}
