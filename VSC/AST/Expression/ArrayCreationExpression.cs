using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ArrayCreationExpression : Expression
    {
 			public TypeExpression _type_expression;
			public ExpressionList _expression_list;
			public OptRankSpecifier _opt_rank_specifier;
			public OptArrayInitializer _opt_array_initializer;
			public RankSpecifiers _rank_specifiers;
			public RankSpecifier _rank_specifier;
			public ArrayInitializer _array_initializer;

			[Rule("<array creation expression> ::= new <type expression> '[' <expression list> ']' <opt rank specifier> <opt array initializer>")]
			public ArrayCreationExpression( Semantic _symbol115,TypeExpression _TypeExpression, Semantic _symbol37,ExpressionList _ExpressionList, Semantic _symbol40,OptRankSpecifier _OptRankSpecifier,OptArrayInitializer _OptArrayInitializer)
				{
				_type_expression = _TypeExpression;
				_expression_list = _ExpressionList;
				_opt_rank_specifier = _OptRankSpecifier;
				_opt_array_initializer = _OptArrayInitializer;
				}
			[Rule("<array creation expression> ::= new <type expression> <rank specifiers>")]
			public ArrayCreationExpression( Semantic _symbol115,TypeExpression _TypeExpression,RankSpecifiers _RankSpecifiers)
				{
				_type_expression = _TypeExpression;
				_rank_specifiers = _RankSpecifiers;
				}
			[Rule("<array creation expression> ::= new <rank specifier> <array initializer>")]
			public ArrayCreationExpression( Semantic _symbol115,RankSpecifier _RankSpecifier,ArrayInitializer _ArrayInitializer)
				{
				_rank_specifier = _RankSpecifier;
				_array_initializer = _ArrayInitializer;
				}
}
}
