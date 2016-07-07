using System;
using System.Collections.Generic;
using System.Linq;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// ResolveScope result representing an array creation.
	/// </summary>
	public class ArrayCreateResolveResult : ResolveResult
	{
		/// <summary>
		/// Gets the size arguments.
		/// </summary>
		public readonly IList<ResolveResult> SizeArguments;
		
		/// <summary>
		/// Gets the initializer elements.
		/// This field may be null if no initializer was specified.
		/// </summary>
		public readonly IList<ResolveResult> InitializerElements;
		
		public ArrayCreateResolveResult(IType arrayType, IList<ResolveResult> sizeArguments, IList<ResolveResult> initializerElements)
			: base(arrayType)
		{
			if (sizeArguments == null)
				throw new ArgumentNullException("sizeArguments");
			this.SizeArguments = sizeArguments;
			this.InitializerElements = initializerElements;
		}
		
		public override IEnumerable<ResolveResult> GetChildResults()
		{
			if (InitializerElements != null)
				return SizeArguments.Concat(InitializerElements);
			else
				return SizeArguments;
		}
	}
}
