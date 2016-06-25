using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class VariableDeclarators : Sequence<VariableDeclarator> {
			[Rule("<variable declarators> ::= <variable declarator>")]
			public VariableDeclarators(VariableDeclarator _VariableDeclarator) : base(_VariableDeclarator)
				{
				}
			[Rule("<variable declarators> ::= <variable declarators> <variable declarator>")]
			public VariableDeclarators(VariableDeclarators _VariableDeclarators,VariableDeclarator _VariableDeclarator) : base(_VariableDeclarator,_VariableDeclarators)
				{
				
				}
}
}
