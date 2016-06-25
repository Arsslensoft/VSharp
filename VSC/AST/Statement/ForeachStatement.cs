using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ForeachStatement : Statement
    {
 			public Type _type;
			public Identifier _identifier;
			public Expression _expression;
			public Statement _statement;

			[Rule("<foreach statement> ::= foreach '(' <type> <Identifier> in <expression> ')' <statement>")]
			public ForeachStatement( Semantic _symbol100, Semantic _symbol20,Type _Type,Identifier _Identifier, Semantic _symbol108,Expression _Expression, Semantic _symbol21,Statement _Statement)
				{
				_type = _Type;
				_identifier = _Identifier;
				_expression = _Expression;
				_statement = _Statement;
				}
}
}
