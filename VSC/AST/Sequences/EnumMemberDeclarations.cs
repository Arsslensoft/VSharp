using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class EnumMemberDeclarations : Sequence<EnumMemberDeclaration> {
 			

			[Rule("<enum member declarations> ::= <enum member declaration>")]
			public EnumMemberDeclarations(EnumMemberDeclaration _EnumMemberDeclaration) : base(_EnumMemberDeclaration)
				{
			
				}
			[Rule("<enum member declarations> ::= <enum member declarations> ',' <enum member declaration>")]
			public EnumMemberDeclarations(EnumMemberDeclarations _EnumMemberDeclarations, Semantic _symbol24,EnumMemberDeclaration _EnumMemberDeclaration) : base(_EnumMemberDeclaration,_EnumMemberDeclarations)
				{
			
				}
}
}
