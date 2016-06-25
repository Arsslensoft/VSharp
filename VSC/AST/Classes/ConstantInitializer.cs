using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class ConstantInitializer : Semantic {
 			public ConstantInitializerExpr _constant_initializer_expr;

			[Rule("<constant initializer> ::= '=' <constant initializer expr>")]
			public ConstantInitializer( Semantic _symbol60,ConstantInitializerExpr _ConstantInitializerExpr)
				{
				_constant_initializer_expr = _ConstantInitializerExpr;
				}
}
}
