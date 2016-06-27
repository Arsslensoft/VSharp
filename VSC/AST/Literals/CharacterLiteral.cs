using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class CharacterLiteral : LiteralExpression
    {
 
			[Rule("<Character Constant> ::= CharLiteral")]
			public CharacterLiteral( Semantic _symbol80)
				{
				}
}
}
