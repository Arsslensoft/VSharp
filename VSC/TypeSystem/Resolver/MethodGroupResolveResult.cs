using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{

	/// <summary>
	/// Represents a group of methods.
	/// A method reference used to create a delegate is resolved to a MethodGroupResolveResult.
	/// The MethodGroupResolveResult has no type.
	/// To retrieve the delegate type or the chosen overload, look at the method group conversion.
	/// </summary>
	public class MethodGroupResolveResult : ResolveResult
	{
		readonly IList<MethodListWithDeclaringType> methodLists;
		readonly IList<IType> typeArguments;
		readonly ResolveResult targetResult;
		readonly string methodName;
		
		public MethodGroupResolveResult(ResolveResult targetResult, string methodName, IList<MethodListWithDeclaringType> methods, IList<IType> typeArguments) : base(SpecialTypeSpec.UnknownType)
		{
			if (methods == null)
				throw new ArgumentNullException("methods");
			this.targetResult = targetResult;
			this.methodName = methodName;
			this.methodLists = methods;
			this.typeArguments = typeArguments ?? EmptyList<IType>.Instance;
		}
		
		/// <summary>
		/// Gets the resolve result for the target object.
		/// </summary>
		public ResolveResult TargetResult {
			get { return targetResult; }
		}
		
		/// <summary>
		/// Gets the type of the reference to the target object.
		/// </summary>
		public IType TargetType {
			get { return targetResult != null ? targetResult.Type : SpecialTypeSpec.UnknownType; }
		}
		
		/// <summary>
		/// Gets the name of the methods in this group.
		/// </summary>
		public string MethodName {
			get { return methodName; }
		}
		
		/// <summary>
		/// Gets the methods that were found.
		/// This list does not include extension methods.
		/// </summary>
		public IEnumerable<IMethod> Methods {
			get { return methodLists.SelectMany(m => m.Cast<IMethod>()); }
		}
		
		/// <summary>
		/// Gets the methods that were found, grouped by their declaring type.
		/// This list does not include extension methods.
		/// Base types come first in the list.
		/// </summary>
		public IEnumerable<MethodListWithDeclaringType> MethodsGroupedByDeclaringType {
			get { return methodLists; }
		}
		
		/// <summary>
		/// Gets the type arguments that were explicitly provided.
		/// </summary>
		public IList<IType> TypeArguments {
			get { return typeArguments; }
		}
		
		/// <summary>
		/// List of extension methods, used to avoid re-calculating it in ResolveInvocation() when it was already
		/// calculated by ResolveMemberAccess().
		/// </summary>
		internal List<List<IMethod>> extensionMethods;
		
		// the resolver is used to fetch extension methods on demand
		internal ResolveContext resolver;
		
		/// <summary>
		/// Gets all candidate extension methods.
		/// Note: this includes candidates that are not eligible due to an inapplicable
		/// this argument.
		/// The candidates will only be specialized if the type arguments were provided explicitly.
		/// </summary>
		/// <remarks>
		/// The results are stored in nested lists because they are grouped by using scope.
		/// That is, for "using SomeExtensions; namespace X { using MoreExtensions; ... }",
		/// the return value will be
		/// new List {
		///    new List { all extensions from MoreExtensions },
		///    new List { all extensions from SomeExtensions }
		/// }
		/// </remarks>
		public IEnumerable<IEnumerable<IMethod>> GetExtensionMethods()
		{
			if (resolver != null) {
				Debug.Assert(extensionMethods == null);
				try {
					extensionMethods = resolver.GetExtensionMethods(methodName, typeArguments);
				} finally {
					resolver = null;
				}
			}
			return extensionMethods ?? Enumerable.Empty<IEnumerable<IMethod>>();
		}
		
		/// <summary>
		/// Gets the eligible extension methods.
		/// </summary>
		/// <param name="substituteInferredTypes">
		/// Specifies whether to produce a <see cref="SpecializedMethod"/>
		/// when type arguments could be inferred from <see cref="TargetType"/>.
		/// This setting is only used for inferred types and has no effect if the type parameters are
		/// specified explicitly.
		/// </param>
		/// <remarks>
		/// The results are stored in nested lists because they are grouped by using scope.
		/// That is, for "using SomeExtensions; namespace X { using MoreExtensions; ... }",
		/// the return value will be
		/// new List {
		///    new List { all extensions from MoreExtensions },
		///    new List { all extensions from SomeExtensions }
		/// }
		/// </remarks>
		public IEnumerable<IEnumerable<IMethod>> GetEligibleExtensionMethods(bool substituteInferredTypes)
		{
			var result = new List<List<IMethod>>();
			foreach (var methodGroup in GetExtensionMethods()) {
				var outputGroup = new List<IMethod>();
				foreach (var method in methodGroup) {
					IType[] inferredTypes;
					if (ResolveContext.IsEligibleExtensionMethod(this.TargetType, method, true, out inferredTypes)) {
						if (substituteInferredTypes && inferredTypes != null) {
							outputGroup.Add(method.Specialize(new TypeParameterSubstitution(null, inferredTypes)));
						} else {
							outputGroup.Add(method);
						}
					}
				}
				if (outputGroup.Count > 0)
					result.Add(outputGroup);
			}
			return result;
		}
		
		public override string ToString()
		{
			return string.Format("[{0} with {1} method(s)]", GetType().Name, this.Methods.Count());
		}
		
		public OverloadResolution PerformOverloadResolution(ICompilation compilation, ResolveResult[] arguments, string[] argumentNames = null,
		                                                    bool allowExtensionMethods = true,
		                                                    bool allowExpandingParams = true, 
		                                                    bool allowOptionalParameters = true,
		                                                    bool checkForOverflow = false, VSharpConversions conversions = null)
		{
			
			var typeArgumentArray = this.TypeArguments.ToArray();
			OverloadResolution or = new OverloadResolution(compilation, arguments, argumentNames, typeArgumentArray, conversions);
			or.AllowExpandingParams = allowExpandingParams;
			or.AllowOptionalParameters = allowOptionalParameters;
			or.CheckForOverflow = checkForOverflow;
			
			or.AddMethodLists(methodLists);
			
			if (allowExtensionMethods && !or.FoundApplicableCandidate) {
				// No applicable match found, so let's try extension methods.
				
				var extensionMethods = this.GetExtensionMethods();
				
				if (extensionMethods.Any()) {
				
					ResolveResult[] extArguments = new ResolveResult[arguments.Length + 1];
					extArguments[0] = new ResolveResult(this.TargetType);
					arguments.CopyTo(extArguments, 1);
					string[] extArgumentNames = null;
					if (argumentNames != null) {
						extArgumentNames = new string[argumentNames.Length + 1];
						argumentNames.CopyTo(extArgumentNames, 1);
					}
					var extOr = new OverloadResolution(compilation, extArguments, extArgumentNames, typeArgumentArray, conversions);
					extOr.AllowExpandingParams = allowExpandingParams;
					extOr.AllowOptionalParameters = allowOptionalParameters;
					extOr.IsExtensionMethodInvocation = true;
					extOr.CheckForOverflow = checkForOverflow;
					
					foreach (var g in extensionMethods) {
						foreach (var method in g) 				
						extOr.AddCandidate(method);
						
						if (extOr.FoundApplicableCandidate)
							break;
					}
					// For the lack of a better comparison function (the one within OverloadResolution
					// cannot be used as it depends on the argument set):
					if (extOr.FoundApplicableCandidate || or.BestCandidate == null) {
						// Consider an extension method result better than the normal result only
						// if it's applicable; or if there is no normal result.
						or = extOr;
					}
				}
			}
			
			return or;
		}
		
		public override IEnumerable<ResolveResult> GetChildResults()
		{
			if (targetResult != null)
				return new[] { targetResult };
			else
				return Enumerable.Empty<ResolveResult>();
		}
	}
}
