using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class SwitchSection : Semantic {
 			public SwitchLabels _switch_labels;
			public StatementList _statement_list;

			[Rule("<Switch Section> ::= <Switch Labels> <statement list>")]
			public SwitchSection(SwitchLabels _SwitchLabels,StatementList _StatementList)
				{
				_switch_labels = _SwitchLabels;
				_statement_list = _StatementList;
				}
}
}
