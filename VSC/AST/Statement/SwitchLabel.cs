using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class SwitchLabel : Semantic {
 			public Expression _expression;

			[Rule("<Switch Label> ::= case <expression> ':'")]
			public SwitchLabel( Semantic _symbol77,Expression _Expression, Semantic _symbol28)
				{
				_expression = _Expression;
				}
			[Rule("<Switch Label> ::= default ':'")]
			public SwitchLabel( Semantic _symbol86, Semantic _symbol28)
				{
				}
}
}
