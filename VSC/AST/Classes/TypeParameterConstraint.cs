using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class TypeParameterConstraint : Semantic {
 			public Type _type;

			[Rule("<type parameter constraint> ::= <type>")]
			public TypeParameterConstraint(Type _Type)
				{
				_type = _Type;
				}
			[Rule("<type parameter constraint> ::= class")]
			public TypeParameterConstraint( Semantic _symbol82)
				{
				}
}
}
