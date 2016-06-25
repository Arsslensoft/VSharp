using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptFieldDeclarators : Semantic {
 			public FieldDeclarators _field_declarators;

			[Rule("<opt field declarators> ::= ")]
			public OptFieldDeclarators()
				{
				}
			[Rule("<opt field declarators> ::= <field declarators>")]
			public OptFieldDeclarators(FieldDeclarators _FieldDeclarators)
				{
				_field_declarators = _FieldDeclarators;
				}
}
}
