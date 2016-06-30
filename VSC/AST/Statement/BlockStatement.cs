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

            public override object DoResolve(Context.ResolveContext rc)
            {
                if (_opt_statement_list._statement_list != null)
                    foreach (var stmt in _opt_statement_list._statement_list)
                        stmt.DoResolve(rc);


                return this;
            }
}
}
