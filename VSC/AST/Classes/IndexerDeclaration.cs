using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class IndexerDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public MemberType _member_type;
			public IndexerDeclarationName _indexer_declaration_name;
			public OptFormalParameterList _opt_formal_parameter_list;
			public IndexerBody _indexer_body;

			[Rule("<indexer declaration> ::= <opt attributes> <opt modifiers> <member type> <indexer declaration name> '[' <opt formal parameter list> ']' <indexer body>")]
			public IndexerDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers,MemberType _MemberType,IndexerDeclarationName _IndexerDeclarationName, Semantic _symbol37,OptFormalParameterList _OptFormalParameterList, Semantic _symbol40,IndexerBody _IndexerBody)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_member_type = _MemberType;
				_indexer_declaration_name = _IndexerDeclarationName;
				_opt_formal_parameter_list = _OptFormalParameterList;
				_indexer_body = _IndexerBody;
				}
}
}
