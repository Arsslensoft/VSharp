using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class FieldDeclarators : Sequence<FieldDeclarator> {

			[Rule("<field declarators> ::= <field declarator>")]
			public FieldDeclarators(FieldDeclarator _FieldDeclarator) : base(_FieldDeclarator)
				{
				
				}
			[Rule("<field declarators> ::= <field declarators> <field declarator>")]
			public FieldDeclarators(FieldDeclarators _FieldDeclarators,FieldDeclarator _FieldDeclarator) : base(_FieldDeclarator,_FieldDeclarators)
				{
				
				
				}
}
}
