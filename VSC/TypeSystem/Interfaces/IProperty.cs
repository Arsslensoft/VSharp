using System;

namespace VSC.TypeSystem
{
    /// <summary>
	/// Represents a property or indexer.
	/// </summary>
	public interface IProperty : IParameterizedMember
	{
		bool CanGet { get; }
		bool CanSet { get; }
		
		IMethod Getter { get; }
		IMethod Setter { get; }
		
		bool IsIndexer { get; }
	}
}
