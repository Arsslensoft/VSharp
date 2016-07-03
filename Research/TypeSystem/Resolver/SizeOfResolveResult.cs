using System;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents the 'sizeof'.
	/// </summary>
	public class SizeOfResolveResult : ResolveResult
	{
		readonly IType referencedType;
		readonly int? constantValue;
		
		public SizeOfResolveResult(IType int32, IType referencedType, int? constantValue)
			: base(int32)
		{
			if (referencedType == null)
				throw new ArgumentNullException("referencedType");
			this.referencedType = referencedType;
			this.constantValue = constantValue;
		}
		
		/// <summary>
		/// The type referenced by the 'sizeof'.
		/// </summary>
		public IType ReferencedType {
			get { return referencedType; }
		}

		public override bool IsCompileTimeConstant {
			get {
				return constantValue != null;
			}
		}

		public override object ConstantValue {
			get {
				return constantValue;
			}
		}

		public override bool IsError {
			get {
				return referencedType.IsReferenceType != false;
			}
		}
	}
}
