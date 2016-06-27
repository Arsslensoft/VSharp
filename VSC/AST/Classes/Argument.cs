using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {

    public class NamedArgumentExpression : Semantic
    {
        public Expression _expression;
        public Identifier _identifier;

        [Rule("<named argument expression> ::= <Identifier> ':' <expression>")]
        public NamedArgumentExpression(Identifier ident,Semantic s,Expression _Expression)
        {
            _expression = _Expression;
            _identifier = ident;
        }
       

    }
	public class Argument : Semantic {
 			public Expression _expression;
			public NonSimpleArgument _non_simple_argument;
            public NamedArgumentExpression _named_argument_expression;
			[Rule("<argument> ::= <expression>")]
			public Argument(Expression _Expression)
				{
				_expression = _Expression;
				}
			[Rule("<argument> ::= <non simple argument>")]
			public Argument(NonSimpleArgument _NonSimpleArgument)
				{
				_non_simple_argument = _NonSimpleArgument;
				}
            [Rule("<argument> ::= <named argument expression>")]
            public Argument(NamedArgumentExpression _namedArgument)
            {
                _named_argument_expression = _namedArgument;
            }
}
}
