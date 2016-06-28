using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class CharacterLiteral : LiteralExpression
    {
 
			[Rule("<Character Constant> ::= CharLiteral")]
			public CharacterLiteral( CharTerminal _symbol80)
				{
                  
                    ConstantExpr = new CharConstant(StringHelper.CharFromVSharpLiteral(_symbol80.Name.Substring(1,_symbol80.Name.Length - 2)), _symbol80.position);
				}
}
}
