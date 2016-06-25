using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class IndexerBody : Semantic {
 			public AccessorDeclarations _accessor_declarations;
			public ExpressionBlock _expression_block;

			[Rule("<indexer body> ::= '{' <accessor declarations> '}'")]
			public IndexerBody( Semantic _symbol43,AccessorDeclarations _AccessorDeclarations, Semantic _symbol47)
				{
				_accessor_declarations = _AccessorDeclarations;
				}
			[Rule("<indexer body> ::= <expression block>")]
			public IndexerBody(ExpressionBlock _ExpressionBlock)
				{
				_expression_block = _ExpressionBlock;
				}
}
}