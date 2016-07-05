using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
    /// <summary>
	/// Represents the result of an invocation of a member of a dynamic object.
	/// </summary>
	public class DynamicInvocationResolveResult : ResolveResult
	{
		/// <summary>
		/// Target of the invocation. Can be a dynamic expression or a <see cref="MethodGroupResolveResult"/>.
		/// </summary>
		public readonly ResolveResult Target;

		/// <summary>
		/// Type of the invocation.
		/// </summary>
		public readonly DynamicInvocationType InvocationType;

		/// <summary>
		/// Arguments for the call. Named arguments will be instances of <see cref="NamedArgumentResolveResult"/>.
		/// </summary>
		public readonly IList<ResolveResult> Arguments; 

		/// <summary>
		/// Gets the list of initializer statements that are appplied to the result of this invocation.
		/// This is used to represent object and collection initializers.
		/// With the initializer statements, the <see cref="InitializedObjectResolveResult"/> is used
		/// to refer to the result of this invocation.
		/// Initializer statements can only exist if the <see cref="InvocationType"/> is <see cref="DynamicInvocationType.ObjectCreation"/>.
		/// </summary>
		public readonly IList<ResolveResult> InitializerStatements;

		public DynamicInvocationResolveResult(ResolveResult target, DynamicInvocationType invocationType, IList<ResolveResult> arguments, IList<ResolveResult> initializerStatements = null) : base(SpecialTypeSpec.Dynamic) {
			this.Target                = target;
			this.InvocationType        = invocationType;
			this.Arguments             = arguments ?? EmptyList<ResolveResult>.Instance;
			this.InitializerStatements = initializerStatements ?? EmptyList<ResolveResult>.Instance;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[Dynamic invocation ]");
		}
	}
}
