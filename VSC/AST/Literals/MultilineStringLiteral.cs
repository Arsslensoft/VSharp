using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class MultilineStringLiteral : LiteralExpression
    {
 			public MultilineStringLiteral _multiline_string_constant;

			[Rule("<Multiline String Constant> ::= RegularStringLiteral")]
			[Rule("<Multiline String Constant> ::= VerbatimStringLiteral")]
			public MultilineStringLiteral( Semantic _symbol133)
				{
				}
			[Rule("<Multiline String Constant> ::= RegularStringLiteral <Multiline String Constant>")]
			[Rule("<Multiline String Constant> ::= VerbatimStringLiteral <Multiline String Constant>")]
			public MultilineStringLiteral( Semantic _symbol133,MultilineStringLiteral _MultilineStringConstant)
				{
				_multiline_string_constant = _MultilineStringConstant;
				}
}
}
