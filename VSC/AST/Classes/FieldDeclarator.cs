using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class FieldDeclarator : Semantic {
 			public Identifier _identifier;
			public VariableInitializer _variable_initializer;

			[Rule("<field declarator> ::= ',' <Identifier>")]
			public FieldDeclarator( Semantic _symbol24,Identifier _Identifier)
				{
				_identifier = _Identifier;
				}
			[Rule("<field declarator> ::= ',' <Identifier> '=' <variable initializer>")]
			public FieldDeclarator( Semantic _symbol24,Identifier _Identifier, Semantic _symbol60,VariableInitializer _VariableInitializer)
				{
				_identifier = _Identifier;
				_variable_initializer = _VariableInitializer;
				}
}
}
