using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class TypeDeclarationName : Semantic {
 			public Identifier _identifier;
			public OptTypeParameterList _opt_type_parameter_list;

			[Rule("<type declaration name> ::= <Identifier> <opt type parameter list>")]
			public TypeDeclarationName(Identifier _Identifier,OptTypeParameterList _OptTypeParameterList)
				{
				_identifier = _Identifier;
				_opt_type_parameter_list = _OptTypeParameterList;
				}
}
}
