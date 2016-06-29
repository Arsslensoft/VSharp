using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class BuiltinTypeExpression : Semantic {
 			public BuiltinType _builtin_type;
			public PointerStars _pointer_stars;
            public bool _isnullable = false;
			[Rule("<builtin type expression> ::= <builtin type>")]
			public BuiltinTypeExpression(BuiltinType _BuiltinType)
				{
				_builtin_type = _BuiltinType;
				}
			[Rule("<builtin type expression> ::= <builtin type> '?'")]
			public BuiltinTypeExpression(BuiltinType _BuiltinType, Semantic _symbol32)
				{
                    _builtin_type = _BuiltinType; _isnullable = true;
				}
			[Rule("<builtin type expression> ::= <builtin type> <pointer stars>")]
			public BuiltinTypeExpression(BuiltinType _BuiltinType,PointerStars _PointerStars)
				{
				_builtin_type = _BuiltinType;
				_pointer_stars = _PointerStars;
				}
            public override string ToString()
            {

                string type = _builtin_type._Keyword;
                    if (_isnullable)
                        type += "?";
                    else if (_pointer_stars != null)
                        foreach (var ps in _pointer_stars)
                            type += "*";

                    return type;
                
            }
}
}
