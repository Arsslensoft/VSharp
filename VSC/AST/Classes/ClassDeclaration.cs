using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ClassDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public TypeDeclarationName _type_declaration_name;
			public OptClassBase _opt_class_base;
			public OptTypeParameterConstraintsClauses _opt_type_parameter_constraints_clauses;
			public OptClassMemberDeclarations _opt_class_member_declarations;
			public OptSemicolon _opt_semicolon;

			[Rule("<class declaration> ::= <opt attributes> <opt modifiers> class <type declaration name> <opt class base> <opt type parameter constraints clauses> '{' <opt class member declarations> '}' <opt semicolon>")]
			public ClassDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers, Semantic _symbol82,TypeDeclarationName _TypeDeclarationName,OptClassBase _OptClassBase,OptTypeParameterConstraintsClauses _OptTypeParameterConstraintsClauses, Semantic _symbol43,OptClassMemberDeclarations _OptClassMemberDeclarations, Semantic _symbol47,OptSemicolon _OptSemicolon)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_type_declaration_name = _TypeDeclarationName;
				_opt_class_base = _OptClassBase;
				_opt_type_parameter_constraints_clauses = _OptTypeParameterConstraintsClauses;
				_opt_class_member_declarations = _OptClassMemberDeclarations;
				_opt_semicolon = _OptSemicolon;
				}
}
}
