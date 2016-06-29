using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public enum TypeParameterConstraintKind
    {
        Class,
        Struct,
        Ctor,
        Type
    }
	public class TypeParameterConstraint : Semantic {
 			public Type _type;
            public TypeParameterConstraintKind _kind;
			[Rule("<type parameter constraint> ::= <type>")]
			public TypeParameterConstraint(Type _Type)
				{
                    _type = _Type; _kind = TypeParameterConstraintKind.Type;
				}
             [Rule("<type parameter constraint> ::= struct")]
			[Rule("<type parameter constraint> ::= class")]
			public TypeParameterConstraint( Semantic _symbol82)
				{
                    _kind = _symbol82.Name == "class" ? TypeParameterConstraintKind.Class : TypeParameterConstraintKind.Struct;
				}
             [Rule("<type parameter constraint> ::= self '(' ')'")]
             public TypeParameterConstraint(Semantic _symbol82, Semantic _s1, Semantic _s2)
             {
                 _kind = TypeParameterConstraintKind.Ctor;
             }
}
}
