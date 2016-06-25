using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class TypeParameterConstraints : Sequence<TypeParameterConstraint> {

			[Rule("<type parameter constraints> ::= <type parameter constraint>")]
			public TypeParameterConstraints(TypeParameterConstraint _TypeParameterConstraint) : base(_TypeParameterConstraint)
				{
				}
			[Rule("<type parameter constraints> ::= <type parameter constraints> ',' <type parameter constraint>")]
			public TypeParameterConstraints(TypeParameterConstraints _TypeParameterConstraints, Semantic _symbol24,TypeParameterConstraint _TypeParameterConstraint) : base(_TypeParameterConstraint,_TypeParameterConstraints)
				{
				}
}
}
