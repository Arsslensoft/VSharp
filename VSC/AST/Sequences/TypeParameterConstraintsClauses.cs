using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class TypeParameterConstraintsClauses : Sequence<TypeParameterConstraintsClause> {

			[Rule("<type parameter constraints clauses> ::= <type parameter constraints clause>")]
			public TypeParameterConstraintsClauses(TypeParameterConstraintsClause _TypeParameterConstraintsClause) : base(_TypeParameterConstraintsClause)
				{

				}
			[Rule("<type parameter constraints clauses> ::= <type parameter constraints clauses> <type parameter constraints clause>")]
			public TypeParameterConstraintsClauses(TypeParameterConstraintsClauses _TypeParameterConstraintsClauses,TypeParameterConstraintsClause _TypeParameterConstraintsClause) : base(_TypeParameterConstraintsClause,_TypeParameterConstraintsClauses)
				{


				}
}
}
