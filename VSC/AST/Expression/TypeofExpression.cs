using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class TypeofExpression : Expression
    {
 			public MemberType _member_type;

			[Rule("<typeof expression> ::= typeof '(' <member type> ')'")]
			public TypeofExpression( Semantic _symbol152, Semantic _symbol20,MemberType _MemberType, Semantic _symbol21)
				{
				_member_type = _MemberType;
				}
}
}
