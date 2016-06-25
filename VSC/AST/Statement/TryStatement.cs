using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class TryStatement : Statement
    {
 			public BlockStatement _block_statement;
			public CatchClauses _catch_clauses;
			public FinallyClauseOpt _finally_clause_opt;

			[Rule("<try statement> ::= try <block statement> <Catch Clauses> <Finally Clause Opt>")]
			public TryStatement( Semantic _symbol151,BlockStatement _BlockStatement,CatchClauses _CatchClauses,FinallyClauseOpt _FinallyClauseOpt)
				{
				_block_statement = _BlockStatement;
				_catch_clauses = _CatchClauses;
				_finally_clause_opt = _FinallyClauseOpt;
				}
}
}
