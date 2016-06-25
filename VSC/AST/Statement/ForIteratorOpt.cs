using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ForIteratorOpt : Semantic {
 			public StatementExpList _statement_exp_list;

			[Rule("<For Iterator Opt> ::= <Statement Exp List>")]
			public ForIteratorOpt(StatementExpList _StatementExpList)
				{
				_statement_exp_list = _StatementExpList;
				}
			[Rule("<For Iterator Opt> ::= ")]
			public ForIteratorOpt()
				{
				}
}
}
