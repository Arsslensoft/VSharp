using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ConditionalExpression : Expression
    {
        public Expression condition;
			public Expression _fexpression;
            public Expression _texpression;
		
			[Rule("<conditional expression> ::= <null coalescing expression> '?' <expression> ':' <expression>")]
			public ConditionalExpression(Expression cond, Semantic _symbol32,Expression _tExpression, Semantic _symbol28,Expression _fExpression)
				{
                    condition = cond;
				_texpression = _tExpression;
				_fexpression = _fExpression;
				}
}
}
