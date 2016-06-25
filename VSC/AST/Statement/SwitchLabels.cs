using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class SwitchLabels : Semantic {
 			public SwitchLabel _switch_label;
			public SwitchLabels _switch_labels;

			[Rule("<Switch Labels> ::= <Switch Label>")]
			public SwitchLabels(SwitchLabel _SwitchLabel)
				{
				_switch_label = _SwitchLabel;
				}
			[Rule("<Switch Labels> ::= <Switch Labels> <Switch Label>")]
			public SwitchLabels(SwitchLabels _SwitchLabels,SwitchLabel _SwitchLabel)
				{
				_switch_labels = _SwitchLabels;
				_switch_label = _SwitchLabel;
				}
}
}
