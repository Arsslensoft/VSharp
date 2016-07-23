using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    //
	// Standard composite pattern
	//
    public abstract class CompositeExpression : Expression
    {
        protected Expression expr;

        protected CompositeExpression(Expression expr)
        {
            this.expr = expr;
            this.loc = expr.Location;
        }
        public override Expression DoResolve(ResolveContext rc)
        {
            if (_resolved)
                return this;
            expr = expr.DoResolve(rc);
            if (expr == null)
                return null;

            ResolvedType = expr.Type;
            _resolved = true;
            eclass = expr.eclass;
            return this;
        }
        public override bool HasConditionalAccess()
        {
            return expr.HasConditionalAccess();
        }
        public override bool IsNull
        {
            get { return expr.IsNull; }
        }
    }
    //
	// Base of expressions used only to narrow resolve flow
	//
    //
	// Base class for the `is' and `as' operators
	//


    //
	// A block of object or collection initializers
	//
    //
	// An object initializer expression
	//
    //
	// A collection initializer expression
	//


    //
	// A base access expression
	//
    //
	// Implements simple new expression 
	//
    //
	// New expression with element/object initializers
	//


    //
	// Array initializer expression, the expression is allowed in
	// variable or field initialization only which makes it tricky as
	// the type has to be infered based on the context either from field
	// type or variable type (think of multiple declarators)
	//

    //
	// Represents an implicitly typed array epxression
	//


    //
	// Unary operators are turned into Indirection expressions
	// after semantic analysis (this is so we can take the address
	// of an indirection).
	//
    //
	//Default value expression
	//

    //
	//   Unary implements unary expressions.
	//


    // This represents a typecast in the source language.
	//


    //
	// A boolean-expression is an expression that yields a result
	// of type bool
	//


    //
	// Implements the `stackalloc' keyword
	//


    //
	// This class is used for compound assignments.
	//
}
