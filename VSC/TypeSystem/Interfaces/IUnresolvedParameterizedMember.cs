using System.Collections.Generic;

namespace VSC.TypeSystem
{
    /// <summary>
    /// Represents a method or property.
    /// </summary>
    public interface IUnresolvedParameterizedMember : IUnresolvedMember
    {
        IList<IUnresolvedParameter> Parameters { get; }
    }
}