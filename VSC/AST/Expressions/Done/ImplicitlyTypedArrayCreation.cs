using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    class ImplicitlyTypedArrayCreation : ArrayCreation
    {
        TypeInference best_type_inference;
        public ImplicitlyTypedArrayCreation(ComposedTypeSpecifier rank, ArrayInitializer initializers, Location loc)
            : base(null, rank, initializers, loc)
        {
        }

        public ImplicitlyTypedArrayCreation(ArrayInitializer initializers, Location loc)
            : base(null, initializers, loc)
        {
        }


        public  override Expression DoResolve (ResolveContext ec)
		{
			if (type != null)
				return this;

			dimensions = rank.Dimension;

			best_type_inference = new TypeInference(ec.compilation, ec.conversions);

			if (!ResolveInitializers (ec))
				return null;
            bool success;
            array_element_type = best_type_inference.GetBestCommonType(Initializers.Elements, out success);
            best_type_inference = null;

            if (!success || array_element_type == null ||
				arguments.Count != rank.Dimension) {
				ec.Report.Error (0, loc,
					"The type of an implicitly typed array cannot be inferred from the initializer. Try specifying array type explicitly");
				return null;
			}

			//
			// At this point we found common base type for all initializer elements
			// but we have to be sure that all static initializer elements are of
			// same type
			//
			UnifyInitializerElement (ec);

            ResolvedType = new ArrayType(ec.compilation, array_element_type, dimensions);
			eclass = ExprClass.Value;
			return this;
		}
        //
        // Converts static initializer only
        //
        void UnifyInitializerElement(ResolveContext ec)
        {
            for (int i = 0; i < array_data.Count; ++i)
            {
                Expression e = array_data[i];
                if (e != null)
                    array_data[i] = ec.Convert(e, array_element_type);
            }
        }
    }
}