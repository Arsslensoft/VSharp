using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class BlockVariableDeclaration : Statement {
 			public Type _type;
			public Identifier _identifier;
			public OptLocalVariableInitializer _opt_local_variable_initializer;
			public OptVariableDeclarators _opt_variable_declarators;
			public ConstVariableInitializer _const_variable_initializer;
			public OptConstDeclarators _opt_const_declarators;

			[Rule("<block variable declaration> ::= <type> <Identifier> <opt local variable initializer> <opt variable declarators> ';'")]
			public BlockVariableDeclaration(Type _Type,Identifier _Identifier,OptLocalVariableInitializer _OptLocalVariableInitializer,OptVariableDeclarators _OptVariableDeclarators, Semantic _symbol31)
				{
				_type = _Type;
				_identifier = _Identifier;
				_opt_local_variable_initializer = _OptLocalVariableInitializer;
				_opt_variable_declarators = _OptVariableDeclarators;
				}
			[Rule("<block variable declaration> ::= const <type> <Identifier> <const variable initializer> <opt const declarators> ';'")]
			public BlockVariableDeclaration( Semantic _symbol83,Type _Type,Identifier _Identifier,ConstVariableInitializer _ConstVariableInitializer,OptConstDeclarators _OptConstDeclarators, Semantic _symbol31)
				{
				_type = _Type;
				_identifier = _Identifier;
				_const_variable_initializer = _ConstVariableInitializer;
				_opt_const_declarators = _OptConstDeclarators;
				}
            public override object DoResolve(Context.ResolveContext rc)
            {
                return base.DoResolve(rc);
            }
}
}
