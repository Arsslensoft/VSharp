using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ForInitOpt : Semantic {
 			public BlockVariableDeclaration _block_variable_declaration;
			public StatementExpList _statement_exp_list;

			[Rule("<For Init Opt> ::= <block variable declaration>")]
			public ForInitOpt(BlockVariableDeclaration _BlockVariableDeclaration)
				{
				_block_variable_declaration = _BlockVariableDeclaration;
				}
			[Rule("<For Init Opt> ::= <Statement Exp List>")]
			public ForInitOpt(StatementExpList _StatementExpList)
				{
				_statement_exp_list = _StatementExpList;
				}
			[Rule("<For Init Opt> ::= ")]
			public ForInitOpt()
				{
				}
}
}
