using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class MethodDeclarationName : Semantic {
 			public TypeDeclarationName _type_declaration_name;
			public ExplicitInterface _explicit_interface;
			public Identifier _identifier;
			public OptTypeParameterList _opt_type_parameter_list;

			[Rule("<method declaration name> ::= <type declaration name>")]
			public MethodDeclarationName(TypeDeclarationName _TypeDeclarationName)
				{
				_type_declaration_name = _TypeDeclarationName;
				}
			[Rule("<method declaration name> ::= <explicit interface> <Identifier> <opt type parameter list>")]
			public MethodDeclarationName(ExplicitInterface _ExplicitInterface,Identifier _Identifier,OptTypeParameterList _OptTypeParameterList)
				{
				_explicit_interface = _ExplicitInterface;
				_identifier = _Identifier;
				_opt_type_parameter_list = _OptTypeParameterList;
				}
}
}
