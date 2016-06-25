using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptClassBase : Semantic {
 			public ClassBase _class_base;

			[Rule("<opt class base> ::= ")]
			public OptClassBase()
				{
				}
			[Rule("<opt class base> ::= <class base>")]
			public OptClassBase(ClassBase _ClassBase)
				{
				_class_base = _ClassBase;
				}
}
}
