using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class GetAccessorDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public BlockOrSemicolon _block_or_semicolon;

			[Rule("<get accessor declaration> ::= <opt attributes> <opt modifiers> get <block or semicolon>")]
			public GetAccessorDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers, Semantic _symbol101,BlockOrSemicolon _BlockOrSemicolon)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_block_or_semicolon = _BlockOrSemicolon;
				}
}
}
