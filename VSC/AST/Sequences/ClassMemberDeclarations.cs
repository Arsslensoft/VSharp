using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ClassMemberDeclarations : Sequence<ClassMemberDeclaration> {

			[Rule("<class member declarations> ::= <class member declaration>")]
			public ClassMemberDeclarations(ClassMemberDeclaration _ClassMemberDeclaration) : base(_ClassMemberDeclaration)
				{
				
				}
			[Rule("<class member declarations> ::= <class member declarations> <class member declaration>")]
			public ClassMemberDeclarations(ClassMemberDeclarations _ClassMemberDeclarations,ClassMemberDeclaration _ClassMemberDeclaration) : base(_ClassMemberDeclaration,_ClassMemberDeclarations)
				{
				
				}
}
}
