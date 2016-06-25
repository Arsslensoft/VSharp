using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptTypeParameterConstraintsClauses : Semantic {
 			public TypeParameterConstraintsClauses _type_parameter_constraints_clauses;

			[Rule("<opt type parameter constraints clauses> ::= ")]
			public OptTypeParameterConstraintsClauses()
				{
				}
			[Rule("<opt type parameter constraints clauses> ::= <type parameter constraints clauses>")]
			public OptTypeParameterConstraintsClauses(TypeParameterConstraintsClauses _TypeParameterConstraintsClauses)
				{
				_type_parameter_constraints_clauses = _TypeParameterConstraintsClauses;
				}
}
}
