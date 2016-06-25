using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ConstantDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public Type _type;
			public Identifier _identifier;
			public ConstantInitializer _constant_initializer;
			public OptConstantDeclarators _opt_constant_declarators;

			[Rule("<constant declaration> ::= <opt attributes> <opt modifiers> const <type> <Identifier> <constant initializer> <opt constant declarators> ';'")]
			public ConstantDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers, Semantic _symbol83,Type _Type,Identifier _Identifier,ConstantInitializer _ConstantInitializer,OptConstantDeclarators _OptConstantDeclarators, Semantic _symbol31)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_type = _Type;
				_identifier = _Identifier;
				_constant_initializer = _ConstantInitializer;
				_opt_constant_declarators = _OptConstantDeclarators;
				}
}
}
