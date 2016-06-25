using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class MemberDeclarationName : Semantic {
 			public MethodDeclarationName _method_declaration_name;

			[Rule("<member declaration name> ::= <method declaration name>")]
			public MemberDeclarationName(MethodDeclarationName _MethodDeclarationName)
				{
				_method_declaration_name = _MethodDeclarationName;
				}
}
}
