using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ConstructorInitializer : Semantic {
 			public OptArgumentList _opt_argument_list;

			[Rule("<constructor initializer> ::= ':' super '(' <opt argument list> ')'")]
			[Rule("<constructor initializer> ::= ':' self '(' <opt argument list> ')'")]
			public ConstructorInitializer( Semantic _symbol28, Semantic _symbol146, Semantic _symbol20,OptArgumentList _OptArgumentList, Semantic _symbol21)
				{
				_opt_argument_list = _OptArgumentList;
				}
}
}
