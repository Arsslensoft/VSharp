using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class BlockStatement : Statement {
 			public OptStatementList _opt_statement_list;
            
			[Rule("<block statement> ::= '{' <opt statement list> '}'")]
			public BlockStatement( Semantic _symbol43,OptStatementList _OptStatementList, Semantic _symbol47)
				{
				_opt_statement_list = _OptStatementList;
				}
}
}
