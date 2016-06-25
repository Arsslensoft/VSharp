using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptEnumMemberDeclarations : Semantic {
 			public EnumMemberDeclarations _enum_member_declarations;

			[Rule("<opt enum member declarations> ::= ")]
			public OptEnumMemberDeclarations()
				{
				}
			[Rule("<opt enum member declarations> ::= <enum member declarations>")]
			public OptEnumMemberDeclarations(EnumMemberDeclarations _EnumMemberDeclarations)
				{
				_enum_member_declarations = _EnumMemberDeclarations;
				}
			[Rule("<opt enum member declarations> ::= <enum member declarations> ','")]
			public OptEnumMemberDeclarations(EnumMemberDeclarations _EnumMemberDeclarations, Semantic _symbol24)
				{
				_enum_member_declarations = _EnumMemberDeclarations;
				}
}
}
