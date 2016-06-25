using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class UsingStatement : Statement
    {
 			public Resource _resource;
			public Statement _statement;

			[Rule("<using statement> ::= using '(' <Resource> ')' <statement>")]
			public UsingStatement( Semantic _symbol159, Semantic _symbol20,Resource _Resource, Semantic _symbol21,Statement _Statement)
				{
				_resource = _Resource;
				_statement = _Statement;
				}
}
}
