using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ConstDeclarators : Sequence<ConstDeclarator> {

			[Rule("<const declarators> ::= <const declarator>")]
        public ConstDeclarators(ConstDeclarator _ConstDeclarator)
            : base(_ConstDeclarator)
				{
				}
			[Rule("<const declarators> ::= <const declarators> <const declarator>")]
			public ConstDeclarators(ConstDeclarators _ConstDeclarators,ConstDeclarator _ConstDeclarator) : base(_ConstDeclarator,_ConstDeclarators)
				{
				}
}
}
