using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class TypeExpressionOrArray : Semantic {
 			public TypeExpression _type_expression;
			public RankSpecifiers _rank_specifiers;

			[Rule("<type expression or array> ::= <type expression>")]
			public TypeExpressionOrArray(TypeExpression _TypeExpression)
				{
				_type_expression = _TypeExpression;
				}
			[Rule("<type expression or array> ::= <type expression> <rank specifiers>")]
			public TypeExpressionOrArray(TypeExpression _TypeExpression,RankSpecifiers _RankSpecifiers)
				{
				_type_expression = _TypeExpression;
				_rank_specifiers = _RankSpecifiers;
				}
            public override string ToString()
            {
                return _type_expression.ToString() + _rank_specifiers.ToString();
            }
}
}
