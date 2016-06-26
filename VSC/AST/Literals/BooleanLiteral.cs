using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class BooleanLiteral : Semantic {
        bool val = false;
        string literal;
			[Rule("<Boolean Constant> ::= true")]
			[Rule("<Boolean Constant> ::= false")]
			public BooleanLiteral( Semantic _symbol150)
				{
                    val = bool.Parse(_symbol150.Name);
                    literal = _symbol150.Name;
				}

        

}
}
