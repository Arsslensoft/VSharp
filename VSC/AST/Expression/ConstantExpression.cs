using System;
using System.Collections.Generic;
using VSC.Base;
using VSC.Base.GoldParser.Parser;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;
namespace VSC.AST {
    public class LiteralExpression : Expression
    {

    }
    public class PrimitiveConstantExpression : ConstantExpression
    {
        public PrimitiveConstantExpression(ITypeReference tref, object val, LineInfo pos) : base(tref,val,pos)
        {
      
        }

        public override ResolveResult Resolve(VSharpResolver resolver)
        {
            return new ConstantResolveResult(Type.Resolve(resolver.CurrentTypeResolveContext), Value);
        }

    }
    public class ConstantExpression : Expression, IConstantValue
    {
        public virtual ResolveResult Resolve(VSharpResolver resolver)
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
                        ResolveResult rr = Resolve(new VSharpResolver(nestedContext));
                        return MapToNewContext(rr, context);
                    }
                }
            }
            // Resolve in current context.
            return Resolve(new VSharpResolver(csContext));
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

        public MultilineStringLiteral _multiline_string_constant;
        public CharacterLiteral _character_constant;
        public IntegralLiteral _integral_constant;
        public BooleanLiteral _boolean_constant;
        public NullLiteral _null_constant;
        public FloatLiteral _float_constant;

       
   
        public ConstantExpression(ITypeReference tref,object val, LineInfo pos)
        {
            type = tref;
            value = val;
            position = pos;
        }

        override public string ToString()
        {
            return this.GetType().Name + " (" + Value.ToString() + ")";
        }
        public virtual object GetValue()
        {
            return value;
        }
        readonly ITypeReference type;
        readonly object value;

        public ITypeReference Type
        {
            get { return type; }
        }

        public object Value
        {
            get { return value; }
        }
    }
}
