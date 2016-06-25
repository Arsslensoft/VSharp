using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OptVariableDeclarators : Semantic {
 			public VariableDeclarators _variable_declarators;

			[Rule("<opt variable declarators> ::= ")]
			public OptVariableDeclarators()
				{
				}
			[Rule("<opt variable declarators> ::= <variable declarators>")]
			public OptVariableDeclarators(VariableDeclarators _VariableDeclarators)
				{
				_variable_declarators = _VariableDeclarators;
				}
}
}
