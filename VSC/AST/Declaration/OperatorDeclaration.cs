using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OperatorDeclaration : Semantic {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public OperatorDeclarator _operator_declarator;
			public MethodBodyExpressionBlock _method_body_expression_block;

			[Rule("<operator declaration> ::= <opt attributes> <opt modifiers> <operator declarator> <method body expression block>")]
			public OperatorDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers,OperatorDeclarator _OperatorDeclarator,MethodBodyExpressionBlock _MethodBodyExpressionBlock)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_operator_declarator = _OperatorDeclarator;
				_method_body_expression_block = _MethodBodyExpressionBlock;
				}
}
}
