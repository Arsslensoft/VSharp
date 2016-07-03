using System;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents a resolve error.
	/// 
	/// Note: some errors are represented by other classes; for example a <see cref="ConversionResolveResult"/> may
	/// be erroneous if the conversion is invalid.
	/// </summary>
	/// <seealso cref="ResolveResult.IsError"/>.
	public class ErrorResolveResult : ResolveResult
	{
		/// <summary>
		/// Gets an ErrorResolveResult instance with <c>Type</c> = <c>SpecialTypeSpec.UnknownType</c>.
		/// </summary>
		public static readonly ErrorResolveResult UnknownError = new ErrorResolveResult(SpecialTypeSpec.UnknownType);
		
		public ErrorResolveResult(IType type) : base(type)
		{
		}
		
		public ErrorResolveResult(IType type, string message, Location location) : base(type)
		{
			this.Message = message;
			this.Location = location;
		}
		
		public override bool IsError {
			get { return true; }
		}
		
		public string Message { get; private set; }

        public Location Location { get; private set; }
	}
}
