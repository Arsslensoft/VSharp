using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class SizeofExpression : Expression
    {
 			public Type _type;

			[Rule("<sizeof expression> ::= sizeof '(' <type> ')'")]
			public SizeofExpression( Semantic _symbol141, Semantic _symbol20,Type _Type, Semantic _symbol21)
				{
				_type = _Type;
				}
}
}
