using System.Collections.Generic;

namespace VSC.TypeSystem
{
    public interface IUnresolvedParameter
    {
        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        string Name { get; }
		
        /// <summary>
        /// Gets the declaration region of the variable.
        /// </summary>
        DomRegion Region { get; }
		
        /// <summary>
        /// Gets the type of the variable.
        /// </summary>
        ITypeReference Type { get; }
		
        /// <summary>
        /// Gets the list of attributes.
        /// </summary>
        IList<IUnresolvedAttribute> Attributes { get; }
		
        /// <summary>
        /// Gets whether this parameter is a V# 'ref' parameter.
        /// </summary>
        bool IsRef { get; }
		
        /// <summary>
        /// Gets whether this parameter is a V# 'out' parameter.
        /// </summary>
        bool IsOut { get; }
		
        /// <summary>
        /// Gets whether this parameter is a V# 'params' parameter.
        /// </summary>
        bool IsParams { get; }

        /// <summary>
        /// Gets whether this parameter is a V# 'self' parameter.
        /// </summary>
        bool IsSelf { get; }
		
        /// <summary>
        /// Gets whether this parameter is optional.
        /// </summary>
        bool IsOptional { get; }
		
        IParameter CreateResolvedParameter(ITypeResolveContext context);
    }
}