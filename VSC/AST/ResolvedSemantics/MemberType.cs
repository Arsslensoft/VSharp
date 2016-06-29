using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class MemberType : Semantic {
 			public TypeExpressionOrArray _type_expression_or_array;

			[Rule("<member type> ::= <type expression or array>")]
			public MemberType(TypeExpressionOrArray _TypeExpressionOrArray)
				{
				_type_expression_or_array = _TypeExpressionOrArray;
				}
			[Rule("<member type> ::= void")]
			public MemberType( Semantic _symbol162)
				{
				}
            public override string ToString()
            {
                if (_type_expression_or_array != null)
                    return _type_expression_or_array.ToString();
                else return "void";
            }
}
}
