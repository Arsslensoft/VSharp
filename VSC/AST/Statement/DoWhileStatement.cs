using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class DoWhileStatement : Statement
    {
 			public Statement _normal_statement;
			public Expression _expression;

			[Rule("<do while statement> ::= do <Normal Statement> while '(' <expression> ')' ';'")]
			public DoWhileStatement( Semantic _symbol89,Statement _NormalStatement, Semantic _symbol164, Semantic _symbol20,Expression _Expression, Semantic _symbol21, Semantic _symbol31)
				{
				_normal_statement = _NormalStatement;
				_expression = _Expression;
				}
}
}
