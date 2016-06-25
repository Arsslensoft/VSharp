using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ForStatement : Statement
    {
 			public ForInitOpt _for_init_opt;
			public ForConditionOpt _for_condition_opt;
			public ForIteratorOpt _for_iterator_opt;
			public Statement _statement;

			[Rule("<for statement> ::= for '(' <For Init Opt> ~';' <For Condition Opt> ~';' <For Iterator Opt> ')' <statement>")]
			public ForStatement( Semantic _symbol99, Semantic _symbol20,ForInitOpt _ForInitOpt,ForConditionOpt _ForConditionOpt, ForIteratorOpt _ForIteratorOpt, Semantic _symbol21,Statement _Statement)
				{
				_for_init_opt = _ForInitOpt;
				_for_condition_opt = _ForConditionOpt;
				_for_iterator_opt = _ForIteratorOpt;
				_statement = _Statement;
				}
}
}