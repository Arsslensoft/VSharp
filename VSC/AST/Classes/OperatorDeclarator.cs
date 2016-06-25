using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OperatorDeclarator : Semantic {
 			public OperatorType _operator_type;
			public OverloadableOperator _overloadable_operator;
			public OptFormalParameterList _opt_formal_parameter_list;
			public ConversionOperatorDeclarator _conversion_operator_declarator;

			[Rule("<operator declarator> ::= <operator type> operator <overloadable operator> '(' <opt formal parameter list> ')'")]
			public OperatorDeclarator(OperatorType _OperatorType, Semantic _symbol119,OverloadableOperator _OverloadableOperator, Semantic _symbol20,OptFormalParameterList _OptFormalParameterList, Semantic _symbol21)
				{
				_operator_type = _OperatorType;
				_overloadable_operator = _OverloadableOperator;
				_opt_formal_parameter_list = _OptFormalParameterList;
				}
			[Rule("<operator declarator> ::= <conversion operator declarator>")]
			public OperatorDeclarator(ConversionOperatorDeclarator _ConversionOperatorDeclarator)
				{
				_conversion_operator_declarator = _ConversionOperatorDeclarator;
				}
}
}
