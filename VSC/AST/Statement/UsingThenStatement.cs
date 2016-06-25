using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class UsingThenStatement : Statement
    {
 			public Resource _resource;
			public Statement _then_statement;

			[Rule("<using then statement> ::= using '(' <Resource> ')' <Then Statement>")]
            public UsingThenStatement(Semantic _symbol159, Semantic _symbol20, Resource _Resource, Semantic _symbol21, Statement _Statement)
				{
				_resource = _Resource;
				_then_statement = _Statement;
				}
}
}
