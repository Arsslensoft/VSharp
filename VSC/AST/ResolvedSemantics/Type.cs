using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class Type : Semantic {
 			public TypeExpressionOrArray _type_expression_or_array;

			[Rule("<type> ::= <type expression or array>")]
			public Type(TypeExpressionOrArray _TypeExpressionOrArray)
				{
				_type_expression_or_array = _TypeExpressionOrArray;
				}
            public override string ToString()
            {
                return _type_expression_or_array.ToString();
            }
}
}
