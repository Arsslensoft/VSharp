using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {
    public class ExpressionStatement : Statement
    {
 			public InvocationExpression _invocation_expression;
			public PostIncrementExpression _post_increment_expression;
			public PostDecrementExpression _post_decrement_expression;
			public PreIncrementExpression _pre_increment_expression;
			public PreDecrementExpression _pre_decrement_expression;
			public AssignmentExpression _assignment_expression;

			[Rule("<Expression Statement> ::= <invocation expression> ';'")]
			public ExpressionStatement(InvocationExpression _InvocationExpression, Semantic _symbol31)
				{
				_invocation_expression = _InvocationExpression;
				}
			[Rule("<Expression Statement> ::= <post increment expression> ';'")]
			public ExpressionStatement(PostIncrementExpression _PostIncrementExpression, Semantic _symbol31)
				{
				_post_increment_expression = _PostIncrementExpression;
				}
			[Rule("<Expression Statement> ::= <post decrement expression> ';'")]
			public ExpressionStatement(PostDecrementExpression _PostDecrementExpression, Semantic _symbol31)
				{
				_post_decrement_expression = _PostDecrementExpression;
				}
			[Rule("<Expression Statement> ::= <pre increment expression> ';'")]
			public ExpressionStatement(PreIncrementExpression _PreIncrementExpression, Semantic _symbol31)
				{
				_pre_increment_expression = _PreIncrementExpression;
				}
			[Rule("<Expression Statement> ::= <pre decrement expression> ';'")]
			public ExpressionStatement(PreDecrementExpression _PreDecrementExpression, Semantic _symbol31)
				{
				_pre_decrement_expression = _PreDecrementExpression;
				}
			[Rule("<Expression Statement> ::= <assignment expression> ';'")]
			public ExpressionStatement(AssignmentExpression _AssignmentExpression, Semantic _symbol31)
				{
				_assignment_expression = _AssignmentExpression;
				}
}
}
