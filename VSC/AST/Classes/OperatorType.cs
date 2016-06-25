using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OperatorType : Semantic {
 			public TypeExpressionOrArray _type_expression_or_array;

			[Rule("<operator type> ::= <type expression or array>")]
			public OperatorType(TypeExpressionOrArray _TypeExpressionOrArray)
				{
				_type_expression_or_array = _TypeExpressionOrArray;
				}
			[Rule("<operator type> ::= void")]
			public OperatorType( Semantic _symbol162)
				{
				}
}
}
