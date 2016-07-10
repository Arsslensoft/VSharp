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
		
        /// <summary>
        /// Resolves the specified expression and processes the conversion to targetType.
        /// </summary>
      public  void ResolveAndProcessConversion(Expression expr, IType targetType)
        {
            expr.DoResolve(this);
            if (targetType.Kind != TypeKind.Unknown)
                 ProcessConversion(expr, expr.GetResolveResult(this), targetType);
            
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
        void ProcessConversionResult(Expression expr, ConversionResolveResult rr)
        {
            if (rr != null && !(rr is CastResolveResult))
                ProcessConversion(expr, rr.Input, rr.Conversion, rr.Type);
        }
        /// <summary>
        /// Convert 'rr' to the target type using the specified conversion.
        /// </summary>
        void ProcessConversion(Expression expr, ResolveResult rr, Conversion conversion, IType targetType)
        {
            //AnonymousFunctionConversion afc = conversion as AnonymousFunctionConversion;
            //if (afc != null)
            //{
            //    if (afc.Hypothesis != null)
            //        afc.Hypothesis.MergeInto(this, afc.ReturnType);
            //    if (afc.ExplicitlyTypedLambda != null)
            //        afc.ExplicitlyTypedLambda.ApplyReturnType(this, afc.ReturnType);
        
            //}
            if (expr != null && !expr.IsNull && conversion != Conversion.IdentityConversion)
            {
               // navigator.ProcessConversion(expr, rr, conversion, targetType);
                conversionDict[expr] = new ConversionWithTargetType(conversion, targetType);
            }
        }
    }
}
