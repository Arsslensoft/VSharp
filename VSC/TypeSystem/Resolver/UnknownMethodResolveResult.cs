using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VSC.TypeSystem.Resolver
{
    /// <summary>
    /// Represents an unknown method.
    /// </summary>
    public class UnknownMethodResolveResult : UnknownMemberResolveResult
    {
        readonly ReadOnlyCollection<IParameter> parameters;
		
        public UnknownMethodResolveResult(IType targetType, string methodName, IEnumerable<IType> typeArguments, IEnumerable<IParameter> parameters)
            : base(targetType, methodName, typeArguments)
        {
            this.parameters = new ReadOnlyCollection<IParameter>(parameters.ToArray());
        }
		
        public ReadOnlyCollection<IParameter> Parameters {
            get { return parameters; }
        }
    }
}