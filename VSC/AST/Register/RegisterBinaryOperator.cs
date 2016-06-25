using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class RegisterBinaryOperator : Semantic {
 
			[Rule("<Register Binary Operator> ::= '=='")]
			[Rule("<Register Binary Operator> ::= '!='")]
			[Rule("<Register Binary Operator> ::= '<='")]
			[Rule("<Register Binary Operator> ::= '>='")]
			[Rule("<Register Binary Operator> ::= '>'")]
			[Rule("<Register Binary Operator> ::= '<'")]
			[Rule("<Register Binary Operator> ::= '+'")]
			[Rule("<Register Binary Operator> ::= '-'")]
			[Rule("<Register Binary Operator> ::= '*'")]
			[Rule("<Register Binary Operator> ::= '/'")]
			[Rule("<Register Binary Operator> ::= '%'")]
			[Rule("<Register Binary Operator> ::= '^'")]
			[Rule("<Register Binary Operator> ::= '&'")]
			[Rule("<Register Binary Operator> ::= '|'")]
			[Rule("<Register Binary Operator> ::= '<<'")]
			[Rule("<Register Binary Operator> ::= '>>'")]
			[Rule("<Register Binary Operator> ::= '<~'")]
			[Rule("<Register Binary Operator> ::= '~>'")]
			public RegisterBinaryOperator( Semantic _symbol62)
				{
				}
}
}
