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
using Expression = VSC.AST.Expression;

namespace VSC.TypeSystem.Resolver
{
     partial class ResolveContext
    {
        readonly Dictionary<AST.Expression, ConversionWithTargetType> conversionDict = new Dictionary<AST.Expression, ConversionWithTargetType>();
     
			
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
		
      
    }
}
