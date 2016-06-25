using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class InterfaceDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public TypeDeclarationName _type_declaration_name;
			public OptClassBase _opt_class_base;
			public OptTypeParameterConstraintsClauses _opt_type_parameter_constraints_clauses;
			public OptInterfaceMemberDeclarations _opt_interface_member_declarations;
			public OptSemicolon _opt_semicolon;

			[Rule("<interface declaration> ::= <opt attributes> <opt modifiers> interface <type declaration name> <opt class base> <opt type parameter constraints clauses> '{' <opt interface member declarations> '}' <opt semicolon>")]
			public InterfaceDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers, Semantic _symbol110,TypeDeclarationName _TypeDeclarationName,OptClassBase _OptClassBase,OptTypeParameterConstraintsClauses _OptTypeParameterConstraintsClauses, Semantic _symbol43,OptInterfaceMemberDeclarations _OptInterfaceMemberDeclarations, Semantic _symbol47,OptSemicolon _OptSemicolon)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_type_declaration_name = _TypeDeclarationName;
				_opt_class_base = _OptClassBase;
				_opt_type_parameter_constraints_clauses = _OptTypeParameterConstraintsClauses;
				_opt_interface_member_declarations = _OptInterfaceMemberDeclarations;
				_opt_semicolon = _OptSemicolon;
				}
}
}
