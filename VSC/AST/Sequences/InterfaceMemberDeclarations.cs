using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class InterfaceMemberDeclarations : Sequence<InterfaceMemberDeclaration> {
 			[Rule("<interface member declarations> ::= <interface member declaration>")]
			public InterfaceMemberDeclarations(InterfaceMemberDeclaration _InterfaceMemberDeclaration) : base(_InterfaceMemberDeclaration)
				{
			
				}
			[Rule("<interface member declarations> ::= <interface member declarations> <interface member declaration>")]
			public InterfaceMemberDeclarations(InterfaceMemberDeclarations _InterfaceMemberDeclarations,InterfaceMemberDeclaration _InterfaceMemberDeclaration) : base(_InterfaceMemberDeclaration,_InterfaceMemberDeclarations)
				{
			
				}
}
}
