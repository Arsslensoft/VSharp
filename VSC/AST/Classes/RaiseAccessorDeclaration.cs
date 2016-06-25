using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class RaiseAccessorDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public BlockOrSemicolon _block_or_semicolon;

			[Rule("<raise accessor declaration> ::= <opt attributes> <opt modifiers> raise <block or semicolon>")]
			public RaiseAccessorDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers, Semantic _symbol129,BlockOrSemicolon _BlockOrSemicolon)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_block_or_semicolon = _BlockOrSemicolon;
				}
}
}
