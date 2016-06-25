using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class LambdaParameter : Semantic {
 			public MemberType _member_type;
			public Identifier _identifier;

			[Rule("<lambda parameter> ::= <member type> <Identifier>")]
			public LambdaParameter(MemberType _MemberType,Identifier _Identifier)
				{
				_member_type = _MemberType;
				_identifier = _Identifier;
				}
}
}
