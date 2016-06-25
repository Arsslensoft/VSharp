using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class Resource : Semantic {
 			public BlockVariableDeclaration _block_variable_declaration;
			public Expression _expression;

			[Rule("<Resource> ::= <block variable declaration>")]
			public Resource(BlockVariableDeclaration _BlockVariableDeclaration)
				{
				_block_variable_declaration = _BlockVariableDeclaration;
				}
			[Rule("<Resource> ::= <expression>")]
			public Resource(Expression _Expression)
				{
				_expression = _Expression;
				}
}
}
