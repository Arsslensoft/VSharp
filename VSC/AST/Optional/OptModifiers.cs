using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptModifiers : Semantic {
 			public Modifiers _modifiers;

			[Rule("<opt modifiers> ::= ")]
			public OptModifiers()
				{
				}
			[Rule("<opt modifiers> ::= <modifiers>")]
			public OptModifiers(Modifiers _Modifiers)
				{
				_modifiers = _Modifiers;
				}
}
}
