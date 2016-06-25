using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class DefaultValueExpression : Expression
    {
 			public Type _type;

			[Rule("<default value expression> ::= default '(' <type> ')'")]
			public DefaultValueExpression( Semantic _symbol86, Semantic _symbol20,Type _Type, Semantic _symbol21)
				{
				_type = _Type;
				}
}
}
