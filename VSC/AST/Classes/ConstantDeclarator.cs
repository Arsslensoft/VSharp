using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ConstantDeclarator : Semantic {
 			public Identifier _identifier;
			public ConstantInitializer _constant_initializer;

			[Rule("<constant declarator> ::= ',' <Identifier> <constant initializer>")]
			public ConstantDeclarator( Semantic _symbol24,Identifier _Identifier,ConstantInitializer _ConstantInitializer)
				{
				_identifier = _Identifier;
				_constant_initializer = _ConstantInitializer;
				}
}
}
