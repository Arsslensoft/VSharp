using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ConstructorBody : Semantic {
 			public BlockStatement _block_statement;

			[Rule("<constructor body> ::= <block statement>")]
			public ConstructorBody(BlockStatement _BlockStatement)
				{
				_block_statement = _BlockStatement;
				}
			[Rule("<constructor body> ::= ';'")]
			public ConstructorBody( Semantic _symbol31)
				{
				}
}
}
