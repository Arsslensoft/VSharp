using System.Collections.Generic;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
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


        public override IConstantValue BuilConstantValue(ResolveContext rc, bool isAttributeConstant)
        {
            var initializer = Initializers;
            // Attributes only allow one-dimensional arrays
            if (isAttributeConstant && initializer != null && dimensions < 2)
            {
                ITypeReference type;
                if (TypeExpression == null)
                    type = null;
                else
                {
                    type = TypeExpression as ITypeReference;
                    ComposedTypeSpecifier sp = rank;
                    while (sp != null)
                    {
                        type =new ArrayTypeReference(type, sp.Dimension);
                        sp = sp.Next;
                    }
           
                   
                }
                Constant[] elements = new Constant[initializer.Elements.Count];
                int pos = 0;
                foreach (Expression expr in initializer.Elements)
                {
                    Constant c = expr.BuilConstantValue(rc, isAttributeConstant) as Constant;
                    if (c == null)
                        return null;
                    elements[pos++] = c;
                }
                return new ConstantArrayCreation(type, elements);
            }
            else
            {
                return null;
            }
        }
    }
}