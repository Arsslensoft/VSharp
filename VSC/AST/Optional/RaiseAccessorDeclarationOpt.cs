using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class RaiseAccessorDeclarationOpt : Semantic {
 			public RaiseAccessorDeclaration _raise_accessor_declaration;

			[Rule("<raise accessor declaration opt> ::= <raise accessor declaration>")]
			public RaiseAccessorDeclarationOpt(RaiseAccessorDeclaration _RaiseAccessorDeclaration)
				{
				_raise_accessor_declaration = _RaiseAccessorDeclaration;
				}
			[Rule("<raise accessor declaration opt> ::= ")]
			public RaiseAccessorDeclarationOpt()
				{
				}
}
}
