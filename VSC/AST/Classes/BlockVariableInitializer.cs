using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class BlockVariableInitializer : Semantic {
 			public VariableInitializer _variable_initializer;
			public TypeExpression _type_expression;
			public Expression _expression;

			[Rule("<block variable initializer> ::= <variable initializer>")]
			public BlockVariableInitializer(VariableInitializer _VariableInitializer)
				{
				_variable_initializer = _VariableInitializer;
				}
			[Rule("<block variable initializer> ::= stackalloc <type expression> '[' <expression> ']'")]
			public BlockVariableInitializer( Semantic _symbol142,TypeExpression _TypeExpression, Semantic _symbol37,Expression _Expression, Semantic _symbol40)
				{
				_type_expression = _TypeExpression;
				_expression = _Expression;
				}
			[Rule("<block variable initializer> ::= stackalloc <type expression>")]
			public BlockVariableInitializer( Semantic _symbol142,TypeExpression _TypeExpression)
				{
				_type_expression = _TypeExpression;
				}
}
}
