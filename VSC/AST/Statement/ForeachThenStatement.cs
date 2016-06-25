using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ForeachThenStatement : Statement
    {
 			public Type _type;
			public Identifier _identifier;
			public Expression _expression;
			public Statement _then_statement;

			[Rule("<foreach then statement> ::= foreach '(' <type> <Identifier> in <expression> ')' <Then Statement>")]
			public ForeachThenStatement( Semantic _symbol100, Semantic _symbol20,Type _Type,Identifier _Identifier, Semantic _symbol108,Expression _Expression, Semantic _symbol21,Statement _ThenStatement)
				{
				_type = _Type;
				_identifier = _Identifier;
				_expression = _Expression;
				_then_statement = _ThenStatement;
				}
}
}
