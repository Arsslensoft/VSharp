using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ExplicitInterface : Semantic {
 			public ExplicitInterface _explicit_interface;
			public Identifier _identifier;
			public OptTypeArgumentList _opt_type_argument_list;

			[Rule("<explicit interface> ::= <explicit interface> <Identifier> <opt type argument list> '.'")]
			public ExplicitInterface(ExplicitInterface _ExplicitInterface,Identifier _Identifier,OptTypeArgumentList _OptTypeArgumentList, Semantic _symbol25)
				{
				_explicit_interface = _ExplicitInterface;
				_identifier = _Identifier;
				_opt_type_argument_list = _OptTypeArgumentList;
				}
			[Rule("<explicit interface> ::= <Identifier> <opt type argument list> '.'")]
			public ExplicitInterface(Identifier _Identifier,OptTypeArgumentList _OptTypeArgumentList, Semantic _symbol25)
				{
				_identifier = _Identifier;
				_opt_type_argument_list = _OptTypeArgumentList;
				}
}
}
