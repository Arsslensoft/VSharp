using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class AsmCodeStatement : Statement {
        public string AsmCode;
			[Rule("<Asm Code Statement> ::= ASMCodeLiteral")]
			public AsmCodeStatement( Semantic _symbol72)
				{
                    AsmCode = _symbol72.Name;
				}
            public override object DoResolve(Context.ResolveContext rc)
            {
                return this;
            }
}
}
