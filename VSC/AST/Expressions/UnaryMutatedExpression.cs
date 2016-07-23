using System;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

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

        public override VSC.AST.Expression DoResolve(VSC.TypeSystem.Resolver.ResolveContext rc)
        {
            UnaryOperatorType op = UnaryOperatorType.PostIncrement;
            if (mode == Mode.PreIncrement)
                op = UnaryOperatorType.PreIncrement;
            else if (mode == Mode.PreDecrement)
                op = UnaryOperatorType.Decrement;
            else if (mode == Mode.PostDecrement)
                op = UnaryOperatorType.PostDecrement;

            expr = expr.DoResolve(rc);
            if (expr == null)
                return null;

            // ++/-- on pointer variables of all types except void*
            if (expr.Type is PointerTypeSpec)
            {
                if (((PointerTypeSpec)expr.Type).ElementType.Kind == TypeKind.Void)
                {
                    rc.Report.Error(0, loc, "The operation in question is undefined on void pointers");
                    return null;
                }
            }
            if (expr.eclass == ExprClass.Variable || expr.eclass == ExprClass.IndexerAccess || expr.eclass == ExprClass.PropertyAccess)
                expr = expr.DoResolveLeftValue(rc, expr);
            else
                rc.Report.Error(0, loc, "The operand of an increment or decrement operator must be a variable, property or indexer");

            operation = new UnaryExpression(op, expr, loc).DoResolve(rc);
            if (operation == null)
                return null;


            
	
            _resolved = true;
            ResolvedType = operation.Type;
            eclass = ExprClass.Value;
            return this;
        }
      
    }
}