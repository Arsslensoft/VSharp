using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class LambdaExpressionBody : Semantic {
 			public Expression _expression;
			public BlockStatement _block_statement;

			[Rule("<lambda expression body> ::= <expression>")]
			public LambdaExpressionBody(Expression _Expression)
				{
				_expression = _Expression;
				}
			[Rule("<lambda expression body> ::= <block statement>")]
			public LambdaExpressionBody(BlockStatement _BlockStatement)
				{
				_block_statement = _BlockStatement;
				}
}
}
