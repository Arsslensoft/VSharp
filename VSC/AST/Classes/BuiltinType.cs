using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class BuiltinType : Semantic {
 			public IntegralType _integral_type;

			[Rule("<builtin type> ::= float")]
			[Rule("<builtin type> ::= double")]
			[Rule("<builtin type> ::= bool")]
			[Rule("<builtin type> ::= object")]
			[Rule("<builtin type> ::= string")]
			public BuiltinType( Semantic _symbol98)
				{
				}
			[Rule("<builtin type> ::= <integral type>")]
			public BuiltinType(IntegralType _IntegralType)
				{
				_integral_type = _IntegralType;
				}
}
}
