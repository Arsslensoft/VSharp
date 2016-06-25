using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptConstDeclarators : Semantic {
 			public ConstDeclarators _const_declarators;

			[Rule("<opt const declarators> ::= ")]
			public OptConstDeclarators()
				{
				}
			[Rule("<opt const declarators> ::= <const declarators>")]
			public OptConstDeclarators(ConstDeclarators _ConstDeclarators)
				{
				_const_declarators = _ConstDeclarators;
				}
}
}
