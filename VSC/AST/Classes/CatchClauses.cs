using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class CatchClauses : Semantic {
 			public CatchClause _catch_clause;
			public CatchClauses _catch_clauses;

			[Rule("<Catch Clauses> ::= <Catch Clause> <Catch Clauses>")]
			public CatchClauses(CatchClause _CatchClause,CatchClauses _CatchClauses)
				{
				_catch_clause = _CatchClause;
				_catch_clauses = _CatchClauses;
				}
			[Rule("<Catch Clauses> ::= ")]
			public CatchClauses()
				{
				}
}
}
