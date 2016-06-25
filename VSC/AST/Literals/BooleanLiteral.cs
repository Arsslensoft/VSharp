using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class BooleanLiteral : Semantic {
 
			[Rule("<Boolean Constant> ::= true")]
			[Rule("<Boolean Constant> ::= false")]
			public BooleanLiteral( Semantic _symbol150)
				{
				}
}
}
