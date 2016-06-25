using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ConversionOperatorDeclarator : Semantic {
 			public Type _type;
			public OptFormalParameterList _opt_formal_parameter_list;

			[Rule("<conversion operator declarator> ::= implicit operator <type> '(' <opt formal parameter list> ')'")]
			[Rule("<conversion operator declarator> ::= explicit operator <type> '(' <opt formal parameter list> ')'")]
			public ConversionOperatorDeclarator( Semantic _symbol106, Semantic _symbol119,Type _Type, Semantic _symbol20,OptFormalParameterList _OptFormalParameterList, Semantic _symbol21)
				{
				_type = _Type;
				_opt_formal_parameter_list = _OptFormalParameterList;
				}
}
}
