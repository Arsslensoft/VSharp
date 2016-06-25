using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class StatementExpList : Semantic {
 			public StatementExpList _statement_exp_list;
			public Expression _expression;

			[Rule("<Statement Exp List> ::= <Statement Exp List> ',' <expression>")]
			public StatementExpList(StatementExpList _StatementExpList, Semantic _symbol24,Expression _Expression)
				{
				_statement_exp_list = _StatementExpList;
				_expression = _Expression;
				}
			[Rule("<Statement Exp List> ::= <expression>")]
			public StatementExpList(Expression _Expression)
				{
				_expression = _Expression;
				}
}
}
