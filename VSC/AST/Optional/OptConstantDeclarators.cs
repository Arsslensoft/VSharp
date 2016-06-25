using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptConstantDeclarators : Semantic {
 			public ConstantDeclarators _constant_declarators;

			[Rule("<opt constant declarators> ::= ")]
			public OptConstantDeclarators()
				{
				}
			[Rule("<opt constant declarators> ::= <constant declarators>")]
			public OptConstantDeclarators(ConstantDeclarators _ConstantDeclarators)
				{
				_constant_declarators = _ConstantDeclarators;
				}
}
}
