using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {

    public class MultilineStringLiteral : LiteralExpression
    {
			[Rule("<Multiline String Constant> ::= RegularStringLiteral")]
            public MultilineStringLiteral(RegularStringTerminal _symbol133)
            {
                ConstantExpr = new StringConstant(GetString(_symbol133.Name),_symbol133.position);
            }
			[Rule("<Multiline String Constant> ::= VerbatimStringLiteral")]
			public MultilineStringLiteral( VerbatimStringTerminal _symbol133)
            {
                ConstantExpr = new StringConstant(GetString(_symbol133.Name.Remove(0,1), true), _symbol133.position);
			}
			[Rule("<Multiline String Constant> ::= RegularStringLiteral <Multiline String Constant>")]
            public MultilineStringLiteral(RegularStringTerminal _symbol133, MultilineStringLiteral _MultilineStringConstant)
            {
                ConstantExpr = new StringConstant(GetString(_symbol133.Name) + _MultilineStringConstant.ConstantExpr.Value, _symbol133.position);
            }
			[Rule("<Multiline String Constant> ::= VerbatimStringLiteral <Multiline String Constant>")]
            public MultilineStringLiteral(VerbatimStringTerminal _symbol133, MultilineStringLiteral _MultilineStringConstant)
				{
                    ConstantExpr = new StringConstant(GetString(_symbol133.Name.Remove(0, 1) , true) + _MultilineStringConstant.ConstantExpr.Value, _symbol133.position);
				}
}
}
