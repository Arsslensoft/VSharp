using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class AdditiveExpression : Expression
    {
 			public AdditiveExpression _additive_expression;
			public MultiplicativeExpression _multiplicative_expression;
			public Type _type;

			[Rule("<additive expression> ::= <additive expression> '+' <multiplicative expression>")]
			[Rule("<additive expression> ::= <additive expression> '-' <multiplicative expression>")]
			public AdditiveExpression(AdditiveExpression _AdditiveExpression, Semantic _symbol51,MultiplicativeExpression _MultiplicativeExpression)
				{
				_additive_expression = _AdditiveExpression;
				_multiplicative_expression = _MultiplicativeExpression;
				}
            [Rule("<additive expression> ::= <additive expression> is <type>")]
			[Rule("<additive expression> ::= <additive expression> as <type>")]
			public AdditiveExpression(AdditiveExpression _AdditiveExpression, Semantic _symbol71,Type _Type)
				{
				_additive_expression = _AdditiveExpression;
				_type = _Type;
				}
		
			[Rule("<additive expression> ::= <multiplicative expression>")]
			public AdditiveExpression(MultiplicativeExpression _MultiplicativeExpression)
				{
				_multiplicative_expression = _MultiplicativeExpression;
				}
}
}
