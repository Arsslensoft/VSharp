using System;

namespace VSC.AST
{
    /// <summary>
    ///   Unary Mutator expressions (pre and post ++ and --)
    /// </summary>
    ///
    /// <remarks>
    ///   UnaryMutator implements ++ and -- expressions.   It derives from
    ///   ExpressionStatement becuase the pre/post increment/decrement
    ///   operators can be used in a statement context.
    ///
    /// FIXME: Idea, we could split this up in two classes, one simpler
    /// for the common case, and one with the extra fields for more complex
    /// classes (indexers require temporary access;  overloaded require method)
    ///
    /// </remarks>
    public class UnaryMutatedExpression : ExpressionStatement
    {
        [Flags]
        public enum Mode : byte {
            IsIncrement    = 0,
            IsDecrement    = 1,
            IsPre          = 0,
            IsPost         = 2,
			
            PreIncrement   = 0,
            PreDecrement   = IsDecrement,
            PostIncrement  = IsPost,
            PostDecrement  = IsPost | IsDecrement
        }

        Mode mode;
        bool is_expr, recurse;

        protected Expression expr;

        // Holds the real operation
        Expression operation;

        public UnaryMutatedExpression(Mode m, Expression e, Location loc)
        {
            mode = m;
            this.loc = loc;
            expr = e;
        }

        public Mode UnaryMutatorMode {
            get {
                return mode;
            }
        }
		
        public Expression Expr {
            get {
                return expr;
            }
        }
    }
}