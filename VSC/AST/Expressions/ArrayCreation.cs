using System;
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
        public ArrayInitializer initializers;
        //
        // The list of Argument types.
        // This is used to construct the `newarray' or constructor signature
        //
        public List<Expression> arguments;

        int num_arguments;
        protected int dimensions;
        protected readonly ComposedTypeSpecifier rank;

        Dictionary<int, int> bounds;

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
        public ArrayCreation(IType arrayType, IList<Expression> sizeArguments, IList<Expression> initializerElements)
			: base(arrayType)
		{
			if (sizeArguments == null)
				throw new ArgumentNullException("sizeArguments");
            arguments = new List<Expression>(sizeArguments);
            num_arguments = arguments.Count;
            this.initializers = new ArrayInitializer( initializerElements, Location.Null);
            ResolvedType = arrayType;
            _resolved = true;
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
        protected IType array_element_type;
        Expression first_emit;
        LocalTemporary first_emit_temp;
        protected List<Expression> array_data;

        
		//
		// Converts `source' to an int, uint, long or ulong.
		//
		protected Expression ConvertExpressionToArrayIndex (ResolveContext ec, Expression source, bool pointerArray = false)
		{
			Expression converted;
			
			using (ec.Set (ResolveContext.Options.CheckedScope))
			{
			    converted = ec.Convert(source, KnownTypeReference.Int32.Resolve(ec));
				if (converted == null)
                    converted = ec.Convert(source, KnownTypeReference.UInt32.Resolve(ec));
				if (converted == null)
                    converted = ec.Convert(source, KnownTypeReference.Int64.Resolve(ec));
				if (converted == null)
                    converted = ec.Convert(source, KnownTypeReference.UInt64.Resolve(ec));

				if (converted == null) {
                    ec.Report.Error(0, loc, "Cannot convert type `{0}' to `{1}'",
                    source.Type.ToString(), KnownTypeReference.Int32.ToString());
					return null;
				}
			}

			if (pointerArray)
				return converted;

			//
			// Only positive constants are allowed at compile time
			//
			Constant c = converted as Constant;
			if (c != null && c.IsNegative)
                ec.Report.Error(0, source.Location, "Cannot create an array with a negative size");

			// No conversion needed to array index
			if (converted.Type.IsKnownType(KnownTypeCode.Int32))
				return converted;

            return new CastExpression( KnownTypeReference.Int32.Resolve(ec), converted).DoResolve(ec);
		}
        bool CheckIndices(ResolveContext ec, ArrayInitializer probe, int idx, bool specified_dims, int child_bounds)
        {
            if (initializers != null && bounds == null)
            {
                //
                // We use this to store all the data values in the order in which we
                // will need to store them in the byte blob later
                //
                array_data = new List<Expression>(probe.Count);
                bounds = new Dictionary<int, int>();
            }

            if (specified_dims)
            {
                Expression a = arguments[idx];
                a = a.DoResolve(ec);
                if (a == null)
                    return false;

                a = ConvertExpressionToArrayIndex(ec, a);
                if (a == null)
                    return false;

                arguments[idx] = a;

                if (initializers != null)
                {
                    Constant c = a as Constant;
                    if (c == null && a is CastExpression)
                        c = ((CastExpression)a).Expr as Constant;

                    if (c == null)
                    {
                        ec.Report.Error(0, a.Location, "A constant value is expected");
                        return false;
                    }

                    int value;
                    try
                    {
                        value = System.Convert.ToInt32(c.GetValue());
                    }
                    catch
                    {
                        ec.Report.Error(0, a.Location, "A constant value is expected");
                        return false;
                    }

                    // TODO: probe.Count does not fit ulong in
                    if (value != probe.Count)
                    {
                        ec.Report.Error(0, loc, "An array initializer of length `{0}' was expected", value.ToString());
                        return false;
                    }

                    bounds[idx] = value;
                }
            }

            if (initializers == null)
                return true;

            for (int i = 0; i < probe.Count; ++i)
            {
                var o = probe[i];
                if (o is ArrayInitializer)
                {
                    var sub_probe = o as ArrayInitializer;
                    if (idx + 1 >= dimensions)
                    {
                        ec.Report.Error(0, loc, "Array initializers can only be used in a variable or field initializer. Try using a new expression instead");
                        return false;
                    }

                    // When we don't have explicitly specified dimensions, record whatever dimension we first encounter at each level
                    if (!bounds.ContainsKey(idx + 1))
                        bounds[idx + 1] = sub_probe.Count;

                    if (bounds[idx + 1] != sub_probe.Count)
                    {
                        ec.Report.Error(0, sub_probe.Location, "An array initializer of length `{0}' was expected", bounds[idx + 1].ToString());
                        return false;
                    }

                    bool ret = CheckIndices(ec, sub_probe, idx + 1, specified_dims, child_bounds - 1);
                    if (!ret)
                        return false;
                }
                else if (child_bounds > 1)
                {
                    ec.Report.Error(0, o.Location, "A nested array initializer was expected");
                }
                else
                {
                    Expression element = ResolveArrayElement(ec, o);
                    if (element == null)
                        continue;
                    array_data.Add(element);
                }
            }

            return true;
        }
        void UpdateIndices(ResolveContext rc)
        {
            int i = 0;
            for (var probe = initializers; probe != null; )
            {
                Expression e = new IntConstant(probe.Count, Location.Null);
                arguments.Add(e);
                bounds[i++] = probe.Count;

                if (probe.Count > 0 && probe[0] is ArrayInitializer)
                {
                    probe = (ArrayInitializer)probe[0];
                }
                else if (dimensions > i)
                {
                    continue;
                }
                else
                {
                    return;
                }
            }
        }
        protected virtual Expression ResolveArrayElement(ResolveContext ec, Expression element)
        {
            element = element.DoResolve(ec);
            if (element == null)
                return null;

            if (element is CompoundAssign.TargetExpression)
            {
                if (first_emit != null)
                    throw new InternalErrorException("Can only handle one mutator at a time");
                first_emit = element;
                element = first_emit_temp = new LocalTemporary(element.Type);
            }

            return new CastExpression(array_element_type, element, loc).DoResolve(ec);
        }
        protected bool ResolveInitializers(ResolveContext ec)
        {
            if (arguments != null)
            {
                bool res = true;
                for (int i = 0; i < arguments.Count; ++i)
                {
                    res &= CheckIndices(ec, initializers, i, true, dimensions);
                    if (initializers != null)
                        break;
                }

                return res;
            }

            arguments = new List<Expression>();

            if (!CheckIndices(ec, initializers, 0, false, dimensions))
                return false;

            UpdateIndices(ec);

            return true;
        }
        //
        // Resolved the type of the array
        //
        bool ResolveArrayType(ResolveContext ec)
        {
            //
            // Lookup the type
            //
            FullNamedExpression array_type_expr;
            if (num_arguments > 0)
                array_type_expr = new ComposedType(requested_base_type, rank);
            else
                array_type_expr = requested_base_type;
     

            ResolvedType = array_type_expr.ResolveAsType(ec);
            if (array_type_expr == null)
                return false;

            var ac = ResolvedType as ArrayType;
            if (ac == null)
            {
                ec.Report.Error(0, loc, "Can only use array initializer expressions to assign to array types. Try using a new expression instead");
                return false;
            }

            array_element_type = ac.ElementType;
            dimensions = ac.Dimensions;
            return true;
        }

        public override Expression DoResolve(ResolveContext ec)
        {
            if (_resolved)
                return this;


            if (type != null)
                return this;

            if (!ResolveArrayType(ec))
                return null;

            //
            // validate the initializers and fill in any missing bits
            //
            if (!ResolveInitializers(ec))
                return null;

            _resolved = true;
            eclass = ExprClass.Value;
            return this;
        }



        //public override IConstantValue BuilConstantValue(bool isAttributeConstant)
        //{
        //    var initializer = Initializers;
        //    // Attributes only allow one-dimensional arrays
        //    if (isAttributeConstant && initializer != null && dimensions < 2)
        //    {
        //        ITypeReference type;
        //        if (TypeExpression == null)
        //            type = null;
        //        else
        //        {
        //            type = TypeExpression as ITypeReference;
        //            ComposedTypeSpecifier sp = rank;
        //            while (sp != null)
        //            {
        //                type =new ArrayTypeReference(type, sp.Dimension);
        //                sp = sp.Next;
        //            }
           
                   
        //        }
        //        Constant[] elements = new Constant[initializer.Elements.Count];
        //        int pos = 0;
        //        foreach (Expression expr in initializer.Elements)
        //        {
        //            Constant c = expr.BuilConstantValue( isAttributeConstant) as Constant;
        //            if (c == null)
        //                return null;
        //            elements[pos++] = c;
        //        }
        //        return new ConstantArrayCreation(type, elements);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
    }
}