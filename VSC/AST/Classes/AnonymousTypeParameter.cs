using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class AnonymousTypeParameter : Semantic {
 			public Identifier _identifier;
			public VariableInitializer _variable_initializer;

			[Rule("<anonymous type parameter> ::= <Identifier> '=' <variable initializer>")]
			public AnonymousTypeParameter(Identifier _Identifier, Semantic _symbol60,VariableInitializer _VariableInitializer)
				{
				_identifier = _Identifier;
				_variable_initializer = _VariableInitializer;
				}
}
}
