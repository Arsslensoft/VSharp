using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptInterfaceMemberDeclarations : Semantic {
 			public DeclaratonSequence _interface_member_declarations;

			[Rule("<opt interface member declarations> ::= ")]
			public OptInterfaceMemberDeclarations()
				{
				}
			[Rule("<opt interface member declarations> ::= <interface member declarations>")]
            public OptInterfaceMemberDeclarations(DeclaratonSequence _InterfaceMemberDeclarations)
				{
				_interface_member_declarations = _InterfaceMemberDeclarations;
				}
}
}
