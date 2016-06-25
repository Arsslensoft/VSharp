using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class MethodDeclaration : Semantic {
 			public MethodHeader _method_header;
			public MethodBodyExpressionBlock _method_body_expression_block;

			[Rule("<method declaration> ::= <method header> <method body expression block>")]
			public MethodDeclaration(MethodHeader _MethodHeader,MethodBodyExpressionBlock _MethodBodyExpressionBlock)
				{
				_method_header = _MethodHeader;
				_method_body_expression_block = _MethodBodyExpressionBlock;
				}
}
}
