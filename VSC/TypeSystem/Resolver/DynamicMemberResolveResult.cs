using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents the result of an access to a member of a dynamic object.
	/// </summary>
	public class DynamicMemberResolveResult : ResolveResult
	{
		/// <summary>
		/// Target of the member access (a dynamic object).
		/// </summary>
		public readonly ResolveResult Target;

		/// <summary>
		/// Name of the accessed member.
		/// </summary>
		public readonly string Member;

		public DynamicMemberResolveResult(ResolveResult target, string member) : base(SpecialTypeSpec.Dynamic) {
			this.Target = target;
			this.Member = member;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[Dynamic member '{0}']", Member);
		}

		public override IEnumerable<ResolveResult> GetChildResults() {
			return new[] { Target };
		}
	}
}
