using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class FinallyClauseOpt : Semantic {
 			public BlockStatement _block_statement;

			[Rule("<Finally Clause Opt> ::= finally <block statement>")]
			public FinallyClauseOpt( Semantic _symbol97,BlockStatement _BlockStatement)
				{
				_block_statement = _BlockStatement;
				}
			[Rule("<Finally Clause Opt> ::= ")]
			public FinallyClauseOpt()
				{
				}
}
}
