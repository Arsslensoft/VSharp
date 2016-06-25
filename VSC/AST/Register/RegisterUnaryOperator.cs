using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class RegisterUnaryOperator : Semantic {
 
			[Rule("<Register Unary Operator> ::= '++'")]
			[Rule("<Register Unary Operator> ::= '--'")]
			[Rule("<Register Unary Operator> ::= '~'")]
			[Rule("<Register Unary Operator> ::= '!'")]
			[Rule("<Register Unary Operator> ::= '?!'")]
			[Rule("<Register Unary Operator> ::= '??'")]
			[Rule("<Register Unary Operator> ::= '&'")]
			[Rule("<Register Unary Operator> ::= '*'")]
			public RegisterUnaryOperator( Semantic _symbol52)
				{
				}
}
}
