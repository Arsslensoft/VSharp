using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class IfStatement : Statement
    {
 			public Expression _expression;
            public Statement _else_statement;
			public Statement _then_statement;

			[Rule("<if statement> ::= if '(' <expression> ')' <statement>")]
			public IfStatement( Semantic _symbol105, Semantic _symbol20,Expression _Expression, Semantic _symbol21,Statement _Statement)
				{
				_expression = _Expression;
                _then_statement = _Statement;
				}
			[Rule("<if statement> ::= if '(' <expression> ')' <Then Statement> else <statement>")]
			public IfStatement( Semantic _symbol105, Semantic _symbol20,Expression _Expression, Semantic _symbol21,Statement _Statement, Semantic _symbol91,Statement _elseStatement)
				{
				_expression = _Expression;
				_then_statement = _Statement;
                _else_statement = _elseStatement;
				}
}
}
