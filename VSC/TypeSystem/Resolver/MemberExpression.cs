using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VSC.AST;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{


    /// <summary>
    /// Used for constants that could not be converted to IConstantValue.
    /// </summary>
    [Serializable]
    public sealed class ErrorConstantValue : IConstantValue
    {
        readonly ITypeReference type;

        public ErrorConstantValue(ITypeReference type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            this.type = type;
        }

        public Expression ResolveConstant(ITypeResolveContext context)
        {
            return new ErrorExpression(type.Resolve(context));
        }
    }
}
