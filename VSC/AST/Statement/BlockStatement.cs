using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class BlockStatement : Statement {
 			public OptStatementList _opt_statement_list;
            public Semantic start;
            public Semantic end;
			[Rule("<block statement> ::= '{' <opt statement list> '}'")]
			public BlockStatement( Semantic _symbol43,OptStatementList _OptStatementList, Semantic _symbol47)
            {
                start = _symbol43;
				_opt_statement_list = _OptStatementList;
                end = _symbol47;
				}
}
}
