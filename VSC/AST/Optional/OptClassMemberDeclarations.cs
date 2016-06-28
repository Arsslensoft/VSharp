using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptClassMemberDeclarations : Semantic {
 			public DeclaratonSequence _class_member_declarations;

			[Rule("<opt class member declarations> ::= ")]
			public OptClassMemberDeclarations()
				{
				}
			[Rule("<opt class member declarations> ::= <class member declarations>")]
            public OptClassMemberDeclarations(DeclaratonSequence _ClassMemberDeclarations)
				{
				_class_member_declarations = _ClassMemberDeclarations;
				}
}
}
