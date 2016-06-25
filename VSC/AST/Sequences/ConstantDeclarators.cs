using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ConstantDeclarators : Sequence<ConstantDeclarator> {
			[Rule("<constant declarators> ::= <constant declarator>")]
			public ConstantDeclarators(ConstantDeclarator _ConstantDeclarator) : base(_ConstantDeclarator)
				{
				
				}
			[Rule("<constant declarators> ::= <constant declarators> <constant declarator>")]
			public ConstantDeclarators(ConstantDeclarators _ConstantDeclarators,ConstantDeclarator _ConstantDeclarator) : base(_ConstantDeclarator,_ConstantDeclarators)
				{
				
				}
}
}
