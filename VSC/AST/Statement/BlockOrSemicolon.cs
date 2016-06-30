using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class BlockOrSemicolon : Statement {
 			public BlockStatement _block_statement;
            public Semantic semicolon;
			[Rule("<block or semicolon> ::= <block statement>")]
			public BlockOrSemicolon(BlockStatement _BlockStatement)
				{
				_block_statement = _BlockStatement;
				}
			[Rule("<block or semicolon> ::= ';'")]
			public BlockOrSemicolon( Semantic _symbol31)
            {
                semicolon = _symbol31
                   ;
				}

            public override object DoResolve(Context.ResolveContext rc)
            {
                if (_block_statement != null)
                    return _block_statement.DoResolve(rc);
                else return new EmptyStatement(semicolon).DoResolve(rc);
            }
}
}
