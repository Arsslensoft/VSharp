using System;
using System.Collections.Generic;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents a named argument.
	/// </summary>
	public class NamedArgumentResolveResult : ResolveResult
	{
		/// <summary>
		/// Gets the member to which the parameter belongs.
		/// This field can be null.
		/// </summary>
		public readonly IParameterizedMember Member;
		
		/// <summary>
		/// Gets the parameter.
		/// This field can be null.
		/// </summary>
		public readonly IParameter Parameter;
		
		/// <summary>
		/// Gets the parameter name.
		/// </summary>
		public readonly string ParameterName;
		
		/// <summary>
		/// Gets the argument passed to the parameter.
		/// </summary>
		public readonly ResolveResult Argument;
		
		public NamedArgumentResolveResult(IParameter parameter, ResolveResult argument, IParameterizedMember member = null)
			: base(argument.Type)
		{
			if (parameter == null)
				throw new ArgumentNullException("parameter");
			if (argument == null)
				throw new ArgumentNullException("argument");
			this.Member = member;
			this.Parameter = parameter;
			this.ParameterName = parameter.Name;
			this.Argument = argument;
		}
		
		public NamedArgumentResolveResult(string parameterName, ResolveResult argument)
			: base(argument.Type)
		{
			if (parameterName == null)
				throw new ArgumentNullException("parameterName");
			if (argument == null)
				throw new ArgumentNullException("argument");
			this.ParameterName = parameterName;
			this.Argument = argument;
		}
		
		public override IEnumerable<ResolveResult> GetChildResults()
		{
			return new [] { Argument };
		}
	}
}
