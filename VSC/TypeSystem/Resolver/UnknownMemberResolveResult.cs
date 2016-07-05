using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using VSC.Base;


namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents an unknown member.
	/// </summary>
	public class UnknownMemberResolveResult : ResolveResult
	{
		readonly IType targetType;
		readonly string memberName;
		readonly ReadOnlyCollection<IType> typeArguments;
		
		public UnknownMemberResolveResult(IType targetType, string memberName, IEnumerable<IType> typeArguments)
			: base(SpecialTypeSpec.UnknownType)
		{
			if (targetType == null)
				throw new ArgumentNullException("targetType");
			this.targetType = targetType;
			this.memberName = memberName;
			this.typeArguments = new ReadOnlyCollection<IType>(typeArguments.ToArray());
		}
		
		/// <summary>
		/// The type on which the method is being called.
		/// </summary>
		public IType TargetType {
			get { return targetType; }
		}
		
		public string MemberName {
			get { return memberName; }
		}
		
		public ReadOnlyCollection<IType> TypeArguments {
			get { return typeArguments; }
		}
		
		public override bool IsError {
			get { return true; }
		}
		
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0} {1}.{2}]", GetType().Name, targetType, memberName);
		}
	}
}
