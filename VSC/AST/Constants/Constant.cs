using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public abstract class Constant : Expression, IConstantValue
    {

        public virtual ResolveResult Resolve(ResolveContext resolver)
        {
            return null;
        }


        public ResolveResult Resolve(ITypeResolveContext context)
        {
            var csContext = (VSharpTypeResolveContext)context;
            if (context.CurrentAssembly != context.Compilation.MainAssembly)
            {
                // The constant needs to be resolved in a different compilation.
                IProjectContent pc = context.CurrentAssembly as IProjectContent;
                if (pc != null)
                {
                    ICompilation nestedCompilation = context.Compilation.SolutionSnapshot.GetCompilation(pc);
                    if (nestedCompilation != null)
                    {
                        var nestedContext = MapToNestedCompilation(csContext, nestedCompilation);
                        ResolveResult rr = Resolve(new ResolveContext(nestedContext));
                        return MapToNewContext(rr, context);
                    }
                }
            }
            // Resolve in current context.
            return Resolve(new ResolveContext(csContext));
        }

        VSharpTypeResolveContext MapToNestedCompilation(VSharpTypeResolveContext context, ICompilation nestedCompilation)
        {
            var nestedContext = new VSharpTypeResolveContext(nestedCompilation.MainAssembly);
            if (context.CurrentUsingScope != null)
            {
                nestedContext = nestedContext.WithUsingScope(context.CurrentUsingScope.UnresolvedUsingScope.Resolve(nestedCompilation));
            }
            if (context.CurrentTypeDefinition != null)
            {
                nestedContext = nestedContext.WithCurrentTypeDefinition(nestedCompilation.Import(context.CurrentTypeDefinition));
            }
            return nestedContext;
        }

        static ResolveResult MapToNewContext(ResolveResult rr, ITypeResolveContext newContext)
        {
            if (rr is TypeOfResolveResult)
            {
                return new TypeOfResolveResult(
                    rr.Type.ToTypeReference().Resolve(newContext),
                    ((TypeOfResolveResult)rr).ReferencedType.ToTypeReference().Resolve(newContext));
            }
            else if (rr is ArrayCreateResolveResult)
            {
                ArrayCreateResolveResult acrr = (ArrayCreateResolveResult)rr;
                return new ArrayCreateResolveResult(
                    acrr.Type.ToTypeReference().Resolve(newContext),
                    MapToNewContext(acrr.SizeArguments, newContext),
                    MapToNewContext(acrr.InitializerElements, newContext));
            }
            else if (rr.IsCompileTimeConstant)
            {
                return new ConstantResolveResult(
                    rr.Type.ToTypeReference().Resolve(newContext),
                    rr.ConstantValue
                );
            }
            else
            {
                return new ErrorResolveResult(rr.Type.ToTypeReference().Resolve(newContext));
            }
        }

        static ResolveResult[] MapToNewContext(IList<ResolveResult> input, ITypeResolveContext newContext)
        {
            if (input == null)
                return null;
            ResolveResult[] output = new ResolveResult[input.Count];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = MapToNewContext(input[i], newContext);
            }
            return output;
        }


        static readonly NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;

        protected Constant(Location loc)
        {
            this.loc = loc;
        }

        override public string ToString()
        {
            return this.GetType().Name + " (" + GetValueAsLiteral() + ")";
        }

        /// <summary>
        ///  This is used to obtain the actual value of the literal
        ///  cast into an object.
        /// </summary>
        public abstract object GetValue();

        public abstract long GetValueAsLong();

        public abstract string GetValueAsLiteral();


        public abstract bool IsDefaultValue
        {
            get;
        }

        public abstract bool IsNegative
        {
            get;
        }

        //
        // When constant is declared as literal
        //
        public virtual bool IsLiteral
        {
            get { return false; }
        }

        public virtual bool IsOneInteger
        {
            get { return false; }
        }

     
        public virtual bool IsZeroInteger
        {
            get { return false; }
        }

    }

    //
	// Null constant can have its own type, think of `default (Foo)'
	//


    //
	// A null constant in a pointer context
	//
}
