using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ConstDeclarator : Semantic {
 			public Identifier _identifier;
			public VariableInitializer _variable_initializer;

			[Rule("<const declarator> ::= ',' <Identifier> '=' <variable initializer>")]
			public ConstDeclarator( Semantic _symbol24,Identifier _Identifier, Semantic _symbol60,VariableInitializer _VariableInitializer)
				{
				_identifier = _Identifier;
				_variable_initializer = _VariableInitializer;
				}
}
}
