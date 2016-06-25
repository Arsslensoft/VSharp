using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class MethodBodyExpressionBlock : Semantic {
 			public BlockOrSemicolon _block_or_semicolon;
			public ExpressionBlock _expression_block;

			[Rule("<method body expression block> ::= <block or semicolon>")]
			public MethodBodyExpressionBlock(BlockOrSemicolon _BlockOrSemicolon)
				{
				_block_or_semicolon = _BlockOrSemicolon;
				}
			[Rule("<method body expression block> ::= <expression block>")]
			public MethodBodyExpressionBlock(ExpressionBlock _ExpressionBlock)
				{
				_expression_block = _ExpressionBlock;
				}
}
}
