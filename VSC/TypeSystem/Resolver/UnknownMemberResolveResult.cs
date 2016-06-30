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
	
	/// <summary>
	/// Represents an unknown identifier.
	/// </summary>
	public class UnknownIdentifierResolveResult : ResolveResult
	{
		readonly string identifier;
		readonly int typeArgumentCount;
		
		public UnknownIdentifierResolveResult(string identifier, int typeArgumentCount = 0)
			: base(SpecialTypeSpec.UnknownType)
		{
			this.identifier = identifier;
			this.typeArgumentCount = typeArgumentCount;
		}
		
		public string Identifier {
			get { return identifier; }
		}
		
		public int TypeArgumentCount {
			get { return typeArgumentCount; }
		}
		
		public override bool IsError {
			get { return true; }
		}
		
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0} {1}]", GetType().Name, identifier);
		}
	}
}
