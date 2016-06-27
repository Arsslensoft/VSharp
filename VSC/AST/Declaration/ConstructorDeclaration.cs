using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ConstructorDeclaration : Semantic {
 			public ConstructorDeclarator _constructor_declarator;
			public ConstructorBody _constructor_body;

			[Rule("<constructor declaration> ::= <constructor declarator> <constructor body>")]
			public ConstructorDeclaration(ConstructorDeclarator _ConstructorDeclarator,ConstructorBody _ConstructorBody)
				{
				_constructor_declarator = _ConstructorDeclarator;
				_constructor_body = _ConstructorBody;
				}
}
}
