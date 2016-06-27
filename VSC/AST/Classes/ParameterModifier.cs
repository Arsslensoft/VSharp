using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ParameterModifier : Semantic {


        public VSC.TypeSystem.ParameterModifier _Modifier;
			[Rule("<parameter modifier> ::= ref")]
			[Rule("<parameter modifier> ::= out")]
			[Rule("<parameter modifier> ::= self")]
			public ParameterModifier( Semantic _symbol132)
				{
                    if (_symbol132.Name == "ref")
                        _Modifier = TypeSystem.ParameterModifier.Ref;
                    else if (_symbol132.Name == "out")
                        _Modifier = TypeSystem.ParameterModifier.Out;
                    else _Modifier = TypeSystem.ParameterModifier.This;
				}
}
}
