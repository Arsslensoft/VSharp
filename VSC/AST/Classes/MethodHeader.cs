using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class MethodHeader : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public MemberType _member_type;
			public MethodDeclarationName _method_declaration_name;
			public OptFormalParameterList _opt_formal_parameter_list;
			public OptTypeParameterConstraintsClauses _opt_type_parameter_constraints_clauses;
			public Modifiers _modifiers;

			[Rule("<method header> ::= <opt attributes> <opt modifiers> <member type> <method declaration name> '(' <opt formal parameter list> ')' <opt type parameter constraints clauses>")]
			public MethodHeader(OptAttributes _OptAttributes,OptModifiers _OptModifiers,MemberType _MemberType,MethodDeclarationName _MethodDeclarationName, Semantic _symbol20,OptFormalParameterList _OptFormalParameterList, Semantic _symbol21,OptTypeParameterConstraintsClauses _OptTypeParameterConstraintsClauses)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_member_type = _MemberType;
				_method_declaration_name = _MethodDeclarationName;
				_opt_formal_parameter_list = _OptFormalParameterList;
				_opt_type_parameter_constraints_clauses = _OptTypeParameterConstraintsClauses;
				}
            [Rule("<method header> ::= <opt attributes> <opt modifiers> <member type> <modifiers> <method declaration name> '(' <opt formal parameter list> ')' <opt type parameter constraints clauses> ")]
            public MethodHeader(OptAttributes _OptAttributes, OptModifiers _OptModifiers, MemberType _MemberType, Modifiers _Modifiers, MethodDeclarationName _MethodDeclarationName, Semantic _symbol20, OptFormalParameterList _OptFormalParameterList, Semantic _symbol21, OptTypeParameterConstraintsClauses _OptTypeParameterConstraintsClauses)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_member_type = _MemberType;
				_modifiers = _Modifiers;
				_method_declaration_name = _MethodDeclarationName;
                _opt_formal_parameter_list = _OptFormalParameterList;
                _opt_type_parameter_constraints_clauses = _OptTypeParameterConstraintsClauses;
				}
}
}
