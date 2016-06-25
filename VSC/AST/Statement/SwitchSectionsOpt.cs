using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class SwitchSectionsOpt : Semantic {
 			public SwitchSectionsOpt _switch_sections_opt;
			public SwitchSection _switch_section;

			[Rule("<Switch Sections Opt> ::= <Switch Sections Opt> <Switch Section>")]
			public SwitchSectionsOpt(SwitchSectionsOpt _SwitchSectionsOpt,SwitchSection _SwitchSection)
				{
				_switch_sections_opt = _SwitchSectionsOpt;
				_switch_section = _SwitchSection;
				}
			[Rule("<Switch Sections Opt> ::= ")]
			public SwitchSectionsOpt()
				{
				}
}
}
