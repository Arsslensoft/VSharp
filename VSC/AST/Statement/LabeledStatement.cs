using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class LabeledStatement : Statement
    {
 			public Identifier _identifier;
			public Statement _statement;

			[Rule("<labeled statement> ::= <Identifier> ':' <statement>")]
			public LabeledStatement(Identifier _Identifier, Semantic _symbol28,Statement _Statement)
				{
				_identifier = _Identifier;
				_statement = _Statement;
				}
}
}
