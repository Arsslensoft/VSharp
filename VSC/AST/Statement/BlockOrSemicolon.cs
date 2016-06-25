using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class BlockOrSemicolon : Semantic {
 			public BlockStatement _block_statement;

			[Rule("<block or semicolon> ::= <block statement>")]
			public BlockOrSemicolon(BlockStatement _BlockStatement)
				{
				_block_statement = _BlockStatement;
				}
			[Rule("<block or semicolon> ::= ';'")]
			public BlockOrSemicolon( Semantic _symbol31)
				{
				}
}
}
