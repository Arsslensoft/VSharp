using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ObjectOrDelegateCreationExpression : Expression
    {
 			public TypeExpression _type_expression;
			public OptArgumentList _opt_argument_list;

			[Rule("<object or delegate creation expression> ::= new <type expression> '(' <opt argument list> ')'")]
			public ObjectOrDelegateCreationExpression( Semantic _symbol115,TypeExpression _TypeExpression, Semantic _symbol20,OptArgumentList _OptArgumentList, Semantic _symbol21)
				{
				_type_expression = _TypeExpression;
				_opt_argument_list = _OptArgumentList;
				}
}
}
