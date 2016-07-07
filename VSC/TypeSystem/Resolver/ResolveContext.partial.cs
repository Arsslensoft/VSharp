using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VSC.AST;
using VSC.Base;
using VSC.TypeSystem.Implementation;

namespace VSC.TypeSystem.Resolver
{
     partial class ResolveContext
    {
         readonly Dictionary<Expression, ConversionWithTargetType> conversionDict = new Dictionary<Expression, ConversionWithTargetType>();
         readonly Dictionary<IAstNode, ResolveResult> resolveResultCache = new Dictionary<IAstNode, ResolveResult>();
         readonly Dictionary<IAstNode, ResolveContext> resolverBeforeDict = new Dictionary<IAstNode, ResolveContext>();
         readonly Dictionary<IAstNode, ResolveContext> resolverAfterDict = new Dictionary<IAstNode, ResolveContext>();

      

         internal struct ConversionWithTargetType
         {
             public readonly Conversion Conversion;
             public readonly IType TargetType;

             public ConversionWithTargetType(Conversion conversion, IType targetType)
             {
                 this.Conversion = conversion;
                 this.TargetType = targetType;
             }
         }

         private IResolveVisitorNavigator navigator = skipAllNavigator;
         static readonly IResolveVisitorNavigator skipAllNavigator = new ConstantModeResolveVisitorNavigator(ResolveVisitorNavigationMode.Skip, null);
		
        /// <summary>
        /// Convert 'rr' to the target type using the specified conversion.
        /// </summary>
        void ProcessConversion(Expression expr, ResolveResult rr, Conversion conversion, IType targetType)
        {
            //AnonymousFunctionConversion afc = conversion as AnonymousFunctionConversion;
            //if (afc != null)
            //{
            //    Log.WriteLine("Processing conversion of anonymous function to " + targetType + "...");

            //    Log.Indent();
            //    if (afc.Hypothesis != null)
            //        afc.Hypothesis.MergeInto(this, afc.ReturnType);
            //    if (afc.ExplicitlyTypedLambda != null)
            //        afc.ExplicitlyTypedLambda.ApplyReturnType(this, afc.ReturnType);
            //    Log.Unindent();
            //}
            if (expr != null && !expr.IsNull && conversion != Conversion.IdentityConversion)
            {
                navigator.ProcessConversion(expr, rr, conversion, targetType);
                conversionDict[expr] = new ConversionWithTargetType(conversion, targetType);
            }
        }

        /// <summary>
        /// Convert 'rr' to the target type.
        /// </summary>
        void ProcessConversion(Expression expr, ResolveResult rr, IType targetType)
        {
            if (expr == null || expr.IsNull)
                return;
            ProcessConversion(expr, rr, conversions.ImplicitConversion(rr, targetType), targetType);
        }
        /// <summary>
        /// Resolves the specified expression and processes the conversion to targetType.
        /// </summary>
        void ResolveAndProcessConversion(Expression expr, IType targetType)
        {
            if (targetType.Kind != TypeKind.Unknown)
                ProcessConversion(expr, expr.GetResolveResult(this), targetType);
            
        }
        void ProcessConversionResult(Expression expr, ConversionResolveResult rr)
        {
            if (rr != null && !(rr is CastResolveResult))
                ProcessConversion(expr, rr.Input, rr.Conversion, rr.Type);
        }
        void ProcessConversionResults(IEnumerable<Expression> expr, IEnumerable<ResolveResult> conversionResolveResults)
        {
            Debug.Assert(expr.Count() == conversionResolveResults.Count());
            using (var e1 = expr.GetEnumerator())
            {
                using (var e2 = conversionResolveResults.GetEnumerator())
                {
                    while (e1.MoveNext() && e2.MoveNext())
                    {
                        ProcessConversionResult(e1.Current, e2.Current as ConversionResolveResult);
                    }
                }
            }
        }

        void StoreCurrentState(IAstNode node)
        {
            // It's possible that we re-visit an expression that we scanned over earlier,
            // so we might have to overwrite an existing state.

#if DEBUG
            ResolveContext oldResolver;
            if (resolverBeforeDict.TryGetValue(node, out oldResolver))
            {
                Debug.Assert(oldResolver.LocalVariables.SequenceEqual(LocalVariables));
            }
#endif

            resolverBeforeDict[node] = this;
        }

        void StoreResult(IAstNode node, ResolveResult result)
        {
            Debug.Assert(result != null);
            if (node != null)
                return;
     
            // Don't use ConversionResolveResult as a result, because it can get
            // confused with an implicit conversion.
            Debug.Assert(!(result is ConversionResolveResult) || result is CastResolveResult);
            resolveResultCache[node] = result;
            if (navigator != null)
                navigator.Resolved(node, result);
        }
        void MarkUnknownNamedArguments(IEnumerable<Argument> arguments)
        {
            foreach (var nae in arguments.OfType<NamedArgument>())
            {
                StoreCurrentState(nae);
                StoreResult(nae, new NamedArgumentResolveResult(nae.Name, resolveResultCache[nae.Expr]));
            }
        }

        void ProcessInvocationResult(Expression target, IEnumerable<Argument> arguments, ResolveResult invocation)
        {
            if (invocation is VSharpInvocationResolveResult || invocation is DynamicInvocationResolveResult)
            {
                int i = 0;
                IList<ResolveResult> argumentsRR;
                if (invocation is VSharpInvocationResolveResult)
                {
                    var csi = (VSharpInvocationResolveResult)invocation;
                    if (csi.IsExtensionMethodInvocation)
                    {
                        Debug.Assert(arguments.Count() + 1 == csi.Arguments.Count);
                        ProcessConversionResult(target, csi.Arguments[0] as ConversionResolveResult);
                        i = 1;
                    }
                    else
                    {
                        Debug.Assert(arguments.Count() == csi.Arguments.Count);
                    }
                    argumentsRR = csi.Arguments;
                }
                else
                {
                    argumentsRR = ((DynamicInvocationResolveResult)invocation).Arguments;
                }

                foreach (Argument arg in arguments)
                {
                    ResolveResult argRR = argumentsRR[i++];
                    NamedArgument nae = arg as NamedArgument;
                    NamedArgumentResolveResult nrr = argRR as NamedArgumentResolveResult;
                    Debug.Assert((nae == null) == (nrr == null));
                    if (nae != null && nrr != null)
                    {
                        StoreCurrentState(nae);
                        StoreResult(nae, nrr);
                        ProcessConversionResult(nae.Expr, nrr.Argument as ConversionResolveResult);
                    }
                    else
                    {
                        ProcessConversionResult(arg.Expr, argRR as ConversionResolveResult);
                    }
                }
            }
            else
            {
                MarkUnknownNamedArguments(arguments);
            }
        }
    }
}
