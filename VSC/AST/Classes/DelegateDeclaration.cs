using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class DelegateDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public MemberType _member_type;
			public TypeDeclarationName _type_declaration_name;
			public OptFormalParameterList _opt_formal_parameter_list;
			public OptTypeParameterConstraintsClauses _opt_type_parameter_constraints_clauses;

			[Rule("<delegate declaration> ::= <opt attributes> <opt modifiers> delegate <member type> <type declaration name> '(' <opt formal parameter list> ')' <opt type parameter constraints clauses> ';'")]
			public DelegateDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers, Semantic _symbol87,MemberType _MemberType,TypeDeclarationName _TypeDeclarationName, Semantic _symbol20,OptFormalParameterList _OptFormalParameterList, Semantic _symbol21,OptTypeParameterConstraintsClauses _OptTypeParameterConstraintsClauses, Semantic _symbol31)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_member_type = _MemberType;
				_type_declaration_name = _TypeDeclarationName;
				_opt_formal_parameter_list = _OptFormalParameterList;
				_opt_type_parameter_constraints_clauses = _OptTypeParameterConstraintsClauses;
				}
}
}
