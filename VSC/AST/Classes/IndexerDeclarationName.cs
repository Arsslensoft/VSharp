using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class IndexerDeclarationName : Semantic {
 			public ExplicitInterface _explicit_interface;

			[Rule("<indexer declaration name> ::= self")]
			public IndexerDeclarationName( Semantic _symbol138)
				{
				}
			[Rule("<indexer declaration name> ::= <explicit interface> self")]
			public IndexerDeclarationName(ExplicitInterface _ExplicitInterface, Semantic _symbol138)
				{
				_explicit_interface = _ExplicitInterface;
				}
}
}
