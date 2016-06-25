using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class Register : Semantic {
 			public Identifier _identifier;

			[Rule("<Register> ::= '$' <Identifier>")]
			public Register( Semantic _symbol13,Identifier _Identifier)
				{
				_identifier = _Identifier;
				}
}
}
