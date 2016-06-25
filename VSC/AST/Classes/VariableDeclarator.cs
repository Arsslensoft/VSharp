using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class VariableDeclarator : Semantic {
 			public Identifier _identifier;
			public BlockVariableInitializer _block_variable_initializer;

			[Rule("<variable declarator> ::= ',' <Identifier>")]
			public VariableDeclarator( Semantic _symbol24,Identifier _Identifier)
				{
				_identifier = _Identifier;
				}
			[Rule("<variable declarator> ::= ',' <Identifier> '=' <block variable initializer>")]
			public VariableDeclarator( Semantic _symbol24,Identifier _Identifier, Semantic _symbol60,BlockVariableInitializer _BlockVariableInitializer)
				{
				_identifier = _Identifier;
				_block_variable_initializer = _BlockVariableInitializer;
				}
}
}
