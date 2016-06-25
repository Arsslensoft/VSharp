using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class SwitchStatement : Statement
    {
 			public Expression _expression;
			public SwitchSectionsOpt _switch_sections_opt;

			[Rule("<switch statement> ::= switch '(' <expression> ')' '{' <Switch Sections Opt> '}'")]
			public SwitchStatement( Semantic _symbol147, Semantic _symbol20,Expression _Expression, Semantic _symbol21, Semantic _symbol43,SwitchSectionsOpt _SwitchSectionsOpt, Semantic _symbol47)
				{
				_expression = _Expression;
				_switch_sections_opt = _SwitchSectionsOpt;
				}
}
}
