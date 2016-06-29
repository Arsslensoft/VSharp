using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ExpressionBlock : Semantic {
 			public Expression _expression;
            public Semantic start;
            public Semantic end;
			[Rule("<expression block> ::= '=>' <expression> ';'")]
			public ExpressionBlock( Semantic _symbol63,Expression _Expression, Semantic _symbol31)
				{
                    start = _symbol63;
                    end = _symbol31;
				_expression = _Expression;
				}
}
}
