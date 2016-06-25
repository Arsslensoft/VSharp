using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class DestructorDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public BlockOrSemicolon _block_or_semicolon;

			[Rule("<destructor declaration> ::= <opt attributes> <opt modifiers> '~' self '(' ')' <block or semicolon>")]
			public DestructorDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers, Semantic _symbol48, Semantic _symbol138, Semantic _symbol20, Semantic _symbol21,BlockOrSemicolon _BlockOrSemicolon)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_block_or_semicolon = _BlockOrSemicolon;
				}
}
}
