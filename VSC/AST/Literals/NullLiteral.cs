using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class NullLiteral : LiteralExpression
    {
 
			[Rule("<Null Constant> ::= null")]
			public NullLiteral( Semantic _symbol116)
				{
				}
}
}
