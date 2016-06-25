using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class AccessorDeclarations : Semantic {
 			public GetAccessorDeclaration _get_accessor_declaration;
			public AccessorDeclarations _accessor_declarations;
			public SetAccessorDeclaration _set_accessor_declaration;

			[Rule("<accessor declarations> ::= <get accessor declaration>")]
			public AccessorDeclarations(GetAccessorDeclaration _GetAccessorDeclaration)
				{
				_get_accessor_declaration = _GetAccessorDeclaration;
				}
			[Rule("<accessor declarations> ::= <get accessor declaration> <accessor declarations>")]
			public AccessorDeclarations(GetAccessorDeclaration _GetAccessorDeclaration,AccessorDeclarations _AccessorDeclarations)
				{
				_get_accessor_declaration = _GetAccessorDeclaration;
				_accessor_declarations = _AccessorDeclarations;
				}
			[Rule("<accessor declarations> ::= <set accessor declaration>")]
			public AccessorDeclarations(SetAccessorDeclaration _SetAccessorDeclaration)
				{
				_set_accessor_declaration = _SetAccessorDeclaration;
				}
			[Rule("<accessor declarations> ::= <set accessor declaration> <accessor declarations>")]
			public AccessorDeclarations(SetAccessorDeclaration _SetAccessorDeclaration,AccessorDeclarations _AccessorDeclarations)
				{
				_set_accessor_declaration = _SetAccessorDeclaration;
				_accessor_declarations = _AccessorDeclarations;
				}
}
}
