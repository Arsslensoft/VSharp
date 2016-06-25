using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ConstructorDeclarator : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public OptFormalParameterList _opt_formal_parameter_list;
			public OptConstructorInitializer _opt_constructor_initializer;

			[Rule("<constructor declarator> ::= <opt attributes> <opt modifiers> self '(' <opt formal parameter list> ')' <opt constructor initializer>")]
			public ConstructorDeclarator(OptAttributes _OptAttributes,OptModifiers _OptModifiers, Semantic _symbol138, Semantic _symbol20,OptFormalParameterList _OptFormalParameterList, Semantic _symbol21,OptConstructorInitializer _OptConstructorInitializer)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_opt_formal_parameter_list = _OptFormalParameterList;
				_opt_constructor_initializer = _OptConstructorInitializer;
				}
}
}
