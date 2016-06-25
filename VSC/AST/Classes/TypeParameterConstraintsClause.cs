using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class TypeParameterConstraintsClause : Semantic {
 			public Identifier _identifier;
			public TypeParameterConstraints _type_parameter_constraints;

			[Rule("<type parameter constraints clause> ::= where <Identifier> ':' <type parameter constraints>")]
			public TypeParameterConstraintsClause( Semantic _symbol163,Identifier _Identifier, Semantic _symbol28,TypeParameterConstraints _TypeParameterConstraints)
				{
				_identifier = _Identifier;
				_type_parameter_constraints = _TypeParameterConstraints;
				}
}
}
