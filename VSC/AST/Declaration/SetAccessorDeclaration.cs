using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class SetAccessorDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public BlockOrSemicolon _block_or_semicolon;

			[Rule("<set accessor declaration> ::= <opt attributes> <opt modifiers> set <block or semicolon>")]
			public SetAccessorDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers, Semantic _symbol139,BlockOrSemicolon _BlockOrSemicolon)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_block_or_semicolon = _BlockOrSemicolon;
				}
}
}
