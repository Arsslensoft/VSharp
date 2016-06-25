using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class CheckedStatement : Statement
    {
 			public BlockStatement _block_statement;

			[Rule("<checked statement> ::= checked <block statement>")]
			public CheckedStatement( Semantic _symbol81,BlockStatement _BlockStatement)
				{
				_block_statement = _BlockStatement;
				}
}
}
