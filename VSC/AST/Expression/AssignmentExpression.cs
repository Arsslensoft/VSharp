using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class AssignmentExpression : Expression
    {
 			public Expression _prefixed_unary_expression;
			public Expression _expression;

			[Rule("<assignment expression> ::= <prefixed unary expression> '=' <expression>")]
			[Rule("<assignment expression> ::= <prefixed unary expression> '*=' <expression>")]
			[Rule("<assignment expression> ::= <prefixed unary expression> '/=' <expression>")]
			[Rule("<assignment expression> ::= <prefixed unary expression> '%=' <expression>")]
			[Rule("<assignment expression> ::= <prefixed unary expression> '+=' <expression>")]
			[Rule("<assignment expression> ::= <prefixed unary expression> '-=' <expression>")]
			[Rule("<assignment expression> ::= <prefixed unary expression> '<<=' <expression>")]
			[Rule("<assignment expression> ::= <prefixed unary expression> '>>=' <expression>")]
			[Rule("<assignment expression> ::= <prefixed unary expression> '<~=' <expression>")]
			[Rule("<assignment expression> ::= <prefixed unary expression> '~>=' <expression>")]
			[Rule("<assignment expression> ::= <prefixed unary expression> '&=' <expression>")]
			[Rule("<assignment expression> ::= <prefixed unary expression> '|=' <expression>")]
			[Rule("<assignment expression> ::= <prefixed unary expression> '^=' <expression>")]
			public AssignmentExpression(Expression _PrefixedUnaryExpression, Semantic _symbol60,Expression _Expression)
				{
				_prefixed_unary_expression = _PrefixedUnaryExpression;
				_expression = _Expression;
				}
}
}
