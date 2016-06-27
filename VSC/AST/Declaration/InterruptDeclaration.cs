using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class InterruptDeclaration : Semantic {
 			public IntegralLiteral _integral_constant;
			public BlockStatement _block_statement;

			[Rule("<interrupt declaration> ::= interrupt <Integral Constant> <block statement>")]
			public InterruptDeclaration( Semantic _symbol112,IntegralLiteral _IntegralConstant,BlockStatement _BlockStatement)
				{
				_integral_constant = _IntegralConstant;
				_block_statement = _BlockStatement;
				}
}
}
