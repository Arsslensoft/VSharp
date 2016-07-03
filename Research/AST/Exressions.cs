using System;
using System.Collections.Generic;
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

    }
    //
	// Base of expressions used only to narrow resolve flow
	//
    public abstract class ShimExpression : Expression
    {
        protected Expression expr;

        protected ShimExpression(Expression expr)
        {
            this.expr = expr;
        }

        public Expression Expr
        {
            get
            {
                return expr;
            }
        }


    }
    public abstract class VariableReference : Expression
    {
        public abstract string Name { get; }
    }
    	/// <summary>
	///   The Assign node takes care of assigning the value of source into
	///   the expression represented by target.
	/// </summary>
    public abstract class Assign : ExpressionStatement
    {
        protected Expression target, source;

        protected Assign(Expression target, Expression source, Location loc)
        {
            this.target = target;
            this.source = source;
            this.loc = loc;
        }

        public Expression Target
        {
            get { return target; }
        }

        public Expression Source
        {
            get
            {
                return source;
            }
        }

    }
    //
	// Base class for the `is' and `as' operators
	//
    public abstract class ProbeExpression : Expression
    {
        public Expression ProbeType;
        protected Expression expr;

        protected ProbeExpression(Expression expr, Expression probe_type, Location l)
        {
            ProbeType = probe_type;
            loc = l;
            this.expr = expr;
        }

        public Expression Expr
        {
            get
            {
                return expr;
            }
        }
    }
  


    //
	// A block of object or collection initializers
	//
    public class CollectionOrObjectInitializers : ExpressionStatement
    {
        IList<Expression> initializers;
        bool is_collection_initialization;

        public CollectionOrObjectInitializers(Location loc)
            : this(new Expression[0], loc)
        {
        }

        public CollectionOrObjectInitializers(IList<Expression> initializers, Location loc)
        {
            this.initializers = initializers;
            this.loc = loc;
        }

        public IList<Expression> Initializers
        {
            get
            {
                return initializers;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return initializers.Count == 0;
            }
        }

        public bool IsCollectionInitializer
        {
            get
            {
                return is_collection_initialization;
            }
        }
    }  
	//
	// An object initializer expression
	//
    public class ElementInitializer : Assign
    {
        public readonly string Name;

        public ElementInitializer(string name, Expression initializer, Location loc)
            : base(null, initializer, loc)
        {
            this.Name = name;
        }
    }
    //
	// A collection initializer expression
	//
    class CollectionElementInitializer : Invocation
    {
        public class ElementInitializerArgument : Argument
        {
            public ElementInitializerArgument(Expression e)
                : base(e, e.Location)
            {
            }
        }
        public CollectionElementInitializer(Expression argument)
            : base(null, new Arguments(1))
        {
            base.arguments.Add(new ElementInitializerArgument(argument));
            this.loc = argument.Location;
        }

        public CollectionElementInitializer(List<Expression> arguments, Location loc)
            : base(null, new Arguments(arguments.Count))
        {
            foreach (Expression e in arguments)
                base.arguments.Add(new ElementInitializerArgument(e));

            this.loc = loc;
        }

        public CollectionElementInitializer(Location loc)
            : base(null, null)
        {
            this.loc = loc;
        }

    }
    public class AnonymousTypeParameter : ShimExpression
    {
        public readonly string Name;

        public AnonymousTypeParameter(Expression initializer, string name, Location loc)
            : base(initializer)
        {
            this.Name = name;
            this.loc = loc;
        }

        public AnonymousTypeParameter(Parameter parameter)
            : base(new SimpleName(parameter.Name, parameter.Location))
        {
            this.Name = parameter.Name;
            this.loc = parameter.Location;
        }


    }


    public class InterpolatedString : Expression
    {
        readonly StringLiteral start, end;
        List<Expression> interpolations;
        Arguments arguments;

        public InterpolatedString(StringLiteral start, List<Expression> interpolations, StringLiteral end)
        {
            this.start = start;
            this.end = end;
            this.interpolations = interpolations;
            loc = start.Location;
        }

    }
    public class InterpolatedStringInsert : CompositeExpression
    {
        public InterpolatedStringInsert(Expression expr)
            : base(expr)
        {
        }

        public Expression Alignment { get; set; }
        public string Format { get; set; }
    }
    public class ParenthesizedExpression : ShimExpression
    {
       public ParenthesizedExpression(Expression expr, Location loc)
            : base(expr)
        {
            this.loc = loc;
        }
    }
    public class ConditionalMemberAccess : MemberAccess
    {
        public ConditionalMemberAccess(Expression expr, string identifier, TypeArguments args, Location loc)
            : base(expr, identifier, args, loc, TypeSystem.Resolver.NameLookupMode.Expression)
        {
        }
    }
    
    /// <summary>
	///   Represents the `self' construct
	/// </summary>
    public class SelfReference : VariableReference
    {

        public SelfReference(Location loc)
        {
            this.loc = loc;
        }

        public override string Name
        {
            get { return "self"; }
        }

      

    }
	//
	// A base access expression
	//
    public class SuperReference : SelfReference
    {
        public SuperReference(Location loc)
            : base(loc)
        {
        }
        public override string Name
        {
            get
            {
                return "super";
            }
        }

    }
   /// <summary>
	///   Invocation of methods or delegates.
	/// </summary>
    public class Invocation : ExpressionStatement
    {
        protected Arguments arguments;
        protected Expression expr;
  

        public Invocation(Expression expr, Arguments arguments)
        {
            this.expr = expr;
            this.arguments = arguments;
            if (expr != null)
            {
                loc = expr.Location;
            }
        }

        public Arguments Arguments
        {
            get
            {
                return arguments;
            }
        }

        public Expression Exp
        {
            get
            {
                return expr;
            }
        }
      
    }
    /// <summary>
	///   Implements checked expressions
	/// </summary>
    public class CheckedExpression : Expression
    {

        public Expression Expr;

        public CheckedExpression(Expression e, Location l)
        {
            Expr = e;
            loc = l;
        }

    }
    /// <summary>
	///   Implements the unchecked expression
	/// </summary>
    public class UnCheckedExpression : Expression
    {

        public Expression Expr;

        public UnCheckedExpression(Expression e, Location l)
        {
            Expr = e;
            loc = l;
        }
    }
    public class DeclarationExpression : Expression
    {
        public DeclarationExpression(FullNamedExpression variableType, LocalVariable variable)
        {
            VariableType = variableType;
            Variable = variable;
            this.loc = variable.Location;
        }

        public LocalVariable Variable { get; set; }
        public Expression Initializer { get; set; }
        public FullNamedExpression VariableType { get; set; }


    }
    /// <summary>
	///   An Element Access expression.
	///
	///   During semantic analysis these are transformed into 
	///   IndexerAccess, ArrayAccess or a PointerArithmetic.
	/// </summary>
    public class ElementAccess : Expression
    {
        public Arguments Arguments;
        public Expression Expr;
        bool conditional_access_receiver;

        public ElementAccess(Expression e, Arguments args, Location loc)
        {
            Expr = e;
            this.loc = loc;
            this.Arguments = args;
        }

        public bool ConditionalAccess { get; set; }

    }
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
    //
	// Implements simple new expression 
	//
    public class NewExpression : ExpressionStatement
    {
        protected Arguments arguments;

        //
        // During bootstrap, it contains the RequestedType,
        // but if `type' is not null, it *might* contain a NewDelegate
        // (because of field multi-initialization)
        //
        protected Expression RequestedType;


        public NewExpression(Expression requested_type, Arguments arguments, Location l)
        {
            RequestedType = requested_type;
            this.arguments = arguments;
            loc = l;
        }

        #region Properties
        public Arguments Arguments
        {
            get
            {
                return arguments;
            }
        }
        public Expression TypeExpression
        {
            get
            {
                return RequestedType;
            }
        }

        #endregion
    }
    public class NewAnonymousTypeExpression : NewExpression
    {
        static readonly AnonymousTypeParameter[] EmptyParameters = new AnonymousTypeParameter[0];

		List<AnonymousTypeParameter> parameters;
        readonly TypeContainer parent;

        public NewAnonymousTypeExpression(List<AnonymousTypeParameter> parameters, TypeContainer parent, Location loc)
			 : base (null, null, loc)
		{
			this.parameters = parameters;
			this.parent = parent;
		}

		public List<AnonymousTypeParameter> Parameters {
			get {
				return this.parameters;
			}
		}
    }
    //
	// New expression with element/object initializers
	//
    public class NewInitializeExpression : NewExpression
    {
        CollectionOrObjectInitializers initializers;
        public NewInitializeExpression(FullNamedExpression requested_type, Arguments arguments, CollectionOrObjectInitializers initializers, Location l)
            : base(requested_type, arguments, l)
        {
            this.initializers = initializers;
        }

        public CollectionOrObjectInitializers Initializers
        {
            get
            {
                return initializers;
            }
        }

    }


    /// <summary>
	///   Represents an array creation expression.
	/// </summary>
	///
	/// <remarks>
	///   There are two possible scenarios here: one is an array creation
	///   expression that specifies the dimensions and optionally the
	///   initialization data and the other which does not need dimensions
	///   specified but where initialization data is mandatory.
	/// </remarks>
    public class ArrayCreation : Expression
    {
        FullNamedExpression requested_base_type;
        ArrayInitializer initializers;
        //
        // The list of Argument types.
        // This is used to construct the `newarray' or constructor signature
        //
        protected List<Expression> arguments;

        int num_arguments;
        protected int dimensions;
        protected readonly ComposedTypeSpecifier rank;


        public ArrayCreation(FullNamedExpression requested_base_type, List<Expression> exprs, ComposedTypeSpecifier rank, ArrayInitializer initializers, Location l)
            : this(requested_base_type, rank, initializers, l)
        {
            arguments = new List<Expression>(exprs);
            num_arguments = arguments.Count;
        }

        //
        // For expressions like int[] foo = new int[] { 1, 2, 3 };
        //
        public ArrayCreation(FullNamedExpression requested_base_type, ComposedTypeSpecifier rank, ArrayInitializer initializers, Location loc)
        {
            this.requested_base_type = requested_base_type;
            this.rank = rank;
            this.initializers = initializers;
            this.loc = loc;

            if (rank != null)
                num_arguments = rank.Dimension;
        }

        //
        // For compiler generated single dimensional arrays only
        //
        public ArrayCreation(FullNamedExpression requested_base_type, ArrayInitializer initializers, Location loc)
            : this(requested_base_type, ComposedTypeSpecifier.SingleDimension, initializers, loc)
        {
        }

        //
        // For expressions like int[] foo = { 1, 2, 3 };
        //
        public ArrayCreation(FullNamedExpression requested_base_type, ArrayInitializer initializers)
            : this(requested_base_type, null, initializers, initializers.Location)
        {
        }

        public ComposedTypeSpecifier Rank
        {
            get
            {
                return this.rank;
            }
        }

        public FullNamedExpression TypeExpression
        {
            get
            {
                return this.requested_base_type;
            }
        }

        public ArrayInitializer Initializers
        {
            get
            {
                return this.initializers;
            }
        }

    }
    	//
	// Array initializer expression, the expression is allowed in
	// variable or field initialization only which makes it tricky as
	// the type has to be infered based on the context either from field
	// type or variable type (think of multiple declarators)
	//
    public class ArrayInitializer : Expression
    {
        List<Expression> elements;
        BlockVariable variable;

        public ArrayInitializer(List<Expression> init, Location loc)
        {
            elements = init;
            this.loc = loc;
        }

        public ArrayInitializer(int count, Location loc)
            : this(new List<Expression>(count), loc)
        {
        }

        public ArrayInitializer(Location loc)
            : this(4, loc)
        {
        }

        #region Properties

        public int Count
        {
            get { return elements.Count; }
        }

        public List<Expression> Elements
        {
            get
            {
                return elements;
            }
        }

        public Expression this[int index]
        {
            get
            {
                return elements[index];
            }
        }

        public BlockVariable VariableDeclaration
        {
            get
            {
                return variable;
            }
            set
            {
                variable = value;
            }
        }

        #endregion

        public void Add(Expression expr)
        {
            elements.Add(expr);
        }
    }

    //
	// Represents an implicitly typed array epxression
	//
    class ImplicitlyTypedArrayCreation : ArrayCreation
    {
        public ImplicitlyTypedArrayCreation(ComposedTypeSpecifier rank, ArrayInitializer initializers, Location loc)
            : base(null, rank, initializers, loc)
        {
        }

        public ImplicitlyTypedArrayCreation(ArrayInitializer initializers, Location loc)
            : base(null, initializers, loc)
        {
        }

    }


    
	/// <summary>
	///   Implements the typeof operator
	/// </summary>
    public class TypeOfExpression : Expression
    {
        FullNamedExpression QueriedType;

        public TypeOfExpression(FullNamedExpression queried_type, Location l)
        {
            QueriedType = queried_type;
            loc = l;
        }


        #region Properties

        public FullNamedExpression TypeExpression
        {
            get
            {
                return QueriedType;
            }
        }

        #endregion

    }
    /// <summary>
	///   Implements the sizeof expression
	/// </summary>
    public class SizeOfExpression : Expression
    {
        readonly Expression texpr;


        public SizeOfExpression(Expression queried_type, Location l)
        {
            this.texpr = queried_type;
            loc = l;
        }
        public Expression TypeExpression
        {
            get
            {
                return texpr;
            }
        }

    }
    //
	// Unary operators are turned into Indirection expressions
	// after semantic analysis (this is so we can take the address
	// of an indirection).
	//
    public class IndirectionExpression : Expression
    {
        Expression expr;
        public IndirectionExpression(Expression expr, Location l)
        {
            this.expr = expr;
            loc = l;
        }

        public Expression Expr
        {
            get
            {
                return expr;
            }
        }


    }
    //
	//Default value expression
	//
    public class DefaultValueExpression : Expression
    {
        Expression expr;

        public DefaultValueExpression(Expression expr, Location loc)
        {
            this.expr = expr;
            this.loc = loc;
        }

        public Expression Expr
        {
            get
            {
                return this.expr;
            }
        }

    }

    //
	//   Unary implements unary expressions.
	//
    public class UnaryExpression : Expression
    {
        	public Expression Expr;
            public readonly VSC.TypeSystem.Resolver.UnaryOperatorType Oper;

      public UnaryExpression(VSC.TypeSystem.Resolver.UnaryOperatorType op, Expression expr, Location loc)
		{
			Oper = op;
			Expr = expr;
			this.loc = loc;
		}
    }
    /// <summary>
	///   Binary operators
	/// </summary>
    public class BinaryExpression : Expression
    {
        readonly VSC.TypeSystem.Resolver.BinaryOperatorType oper;
        Expression left, right;

        public BinaryExpression(VSC.TypeSystem.Resolver.BinaryOperatorType oper, Expression left, Expression right)
            : this(oper, left, right, left.Location)
        {
        }

        public BinaryExpression(VSC.TypeSystem.Resolver.BinaryOperatorType oper, Expression left, Expression right, Location loc)
        {
            this.oper = oper;
            this.left = left;
            this.right = right;
            this.loc = loc;
        }

        #region Properties

        public VSC.TypeSystem.Resolver.BinaryOperatorType Oper
        {
            get
            {
                return oper;
            }
        }

        public Expression Left
        {
            get
            {
                return this.left;
            }
        }

        public Expression Right
        {
            get
            {
                return this.right;
            }
        }



        #endregion
    }
    /// <summary>
	///   Implementation of the `is' operator.
	/// </summary>
    public class IsExpression : ProbeExpression
    {

        public IsExpression(Expression expr, Expression probe_type, Location l)
            : base(expr, probe_type, l)
        {
        }


    }
    /// <summary>
    ///   Implementation of the `as' operator.
    /// </summary>
    public class AsExpression : ProbeExpression
    {

        public AsExpression(Expression expr, Expression probe_type, Location l)
            : base(expr, probe_type, l)
        {
        }


    }


	// This represents a typecast in the source language.
	//
    public class CastExpression : ShimExpression
    {
        Expression target_type;

        public CastExpression(Expression cast_type, Expression expr, Location loc)
            : base(expr)
        {
            this.target_type = cast_type;
            this.loc = loc;
        }

        public Expression TargetType
        {
            get { return target_type; }
        }
    }
  


    //
	// A boolean-expression is an expression that yields a result
	// of type bool
	//
    public class BooleanExpression : ShimExpression
    {
        public BooleanExpression(Expression expr)
            : base(expr)
        {
            this.loc = expr.Location;
        }

    }
    	/// <summary>
	///   Implements the ternary conditional operator (?:)
	/// </summary>
    public class ConditionalExpression : Expression
    {
        Expression expr, true_expr, false_expr;

        public ConditionalExpression(Expression expr, Expression true_expr, Expression false_expr, Location loc)
        {
            this.expr = expr;
            this.true_expr = true_expr;
            this.false_expr = false_expr;
            this.loc = loc;
        }

        #region Properties

        public Expression Expr
        {
            get
            {
                return expr;
            }
        }

        public Expression TrueExpr
        {
            get
            {
                return true_expr;
            }
        }

        public Expression FalseExpr
        {
            get
            {
                return false_expr;
            }
        }

        #endregion
    }













    public class SimpleAssign : Assign
    {
        public SimpleAssign(Expression target, Expression source)
            : this(target, source, target.Location)
        {
        }

        public SimpleAssign(Expression target, Expression source, Location loc)
            : base(target, source, loc)
        {
        }
    }
    //
	// This class is used for compound assignments.
	//
    public class CompoundAssign : Assign
    {
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
