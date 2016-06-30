using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class StatementList : Sequence<Statement>
    {
 		
			[Rule("<statement list> ::= <statement>")]
			public StatementList(Statement _Statement) : base(_Statement)
				{
			
				}
			[Rule("<statement list> ::= <statement list> <statement>")]
			public StatementList(StatementList _StatementList,Statement _Statement) : base(_Statement, _StatementList)
				{
			
				}
}
}
