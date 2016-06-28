using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptModifiers : Semantic {
 			public Modifiers _modifiers;
            public VSC.TypeSystem.Modifiers _Modifiers = TypeSystem.Modifiers.DEFAULT_ACCESS_MODIFIER;
			[Rule("<opt modifiers> ::= ")]
			public OptModifiers()
				{
				}
			[Rule("<opt modifiers> ::= <modifiers>")]
			public OptModifiers(Modifiers _Modifiers)
				{
				_modifiers = _Modifiers;
                this._Modifiers = VSC.TypeSystem.Modifiers.NONE;
                foreach (Modifier m in _Modifiers)
                    this._Modifiers |= m._Modifier;
				}
}
}
