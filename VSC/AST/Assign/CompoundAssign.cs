using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   An Expression to hold a temporary value.
    /// </summary>
    /// <remarks>
    ///   The LocalTemporary class is used to hold temporary values of a given
    ///   type to "simulate" the expression semantics. The local variable is
    ///   never captured.
    ///
    ///   The local temporary is used to alter the normal flow of code generation
    ///   basically it creates a local variable, and its emit instruction generates
    ///   code to access this value, return its address or save its value.
    ///
    ///   If `is_address' is true, then the value that we store is the address to the
    ///   real value, and not the value itself.
    ///
    ///   This is needed for a value type, because otherwise you just end up making a
    ///   copy of the value on the stack and modifying it. You really need a pointer
    ///   to the origional value so that you can modify it in that location. This
    ///   Does not happen with a class because a class is a pointer -- so you always
    ///   get the indirection.
    ///
    /// </remarks>
    public class LocalTemporary : Expression
    {
 

        public LocalTemporary(IType t)
        {
            ResolvedType = t;
            _resolved = true;
            eclass = ExprClass.Value;
        }

     
      
        public override Expression DoResolve(ResolveContext ec)
        {
            return this;
        }

        public override Expression DoResolveLeftValue(ResolveContext ec, Expression right_side)
        {
            return this;
        }



 
    }
    public class CompoundAssign : Assign
    {		// This is just a hack implemented for arrays only

        public sealed class TargetExpression : Expression
        {
            readonly Expression child;

            public TargetExpression(Expression child)
            {
                this.child = child;
                this.loc = child.Location;
            }


            public override Expression DoResolve(ResolveContext ec)
            {
                ResolvedType = child.Type;
                _resolved = true;
                eclass = ExprClass.Value;
                return this;
            }
        }

        // Used for underlying binary operator
        readonly BinaryOperatorType op;
        Expression right;
        Expression left;

        public CompoundAssign(BinaryOperatorType op, Expression target, Expression source)
            : base(target, source, target.Location)
        {
            right = source;
            this.op = op;
        }

        public CompoundAssign(BinaryOperatorType op, Expression target, Expression source, Expression left)
            : this(op, target, source)
        {
            this.left = left;
        }

        public BinaryOperatorType Operator
        {
            get
            {
                return op;
            }
        }

    }
}