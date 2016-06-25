using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class IfThenStatement : Statement
    {
 			public Expression _expression;
			public Statement _then_statement;
            public Statement _else_statement;
			[Rule("<if then statement> ::= if '(' <expression> ')' <Then Statement> else <Then Statement>")]
            public IfThenStatement(Semantic _symbol105, Semantic _symbol20, Expression _Expression, Semantic _symbol21, Statement _Statement, Semantic _symbol91, Statement _elseStatement)
				{
				_expression = _Expression;
				_then_statement = _Statement;
				_else_statement = _elseStatement;
				}
}
}
