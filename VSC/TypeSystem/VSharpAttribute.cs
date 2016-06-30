using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using VSC.Base;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.TypeSystem
{
	[Serializable]
	public sealed class VSharpAttribute : IUnresolvedAttribute
	{
		ITypeReference attributeType;
		DomRegion region;
		IList<IConstantValue> positionalArguments;
		IList<KeyValuePair<string, IConstantValue>> namedCtorArguments;
		IList<KeyValuePair<string, IConstantValue>> namedArguments;

        public VSharpAttribute(ITypeReference attributeType, DomRegion region,
		                       IList<IConstantValue> positionalArguments,
		                       IList<KeyValuePair<string, IConstantValue>> namedCtorArguments,
		                       IList<KeyValuePair<string, IConstantValue>> namedArguments)
		{
			if (attributeType == null)
				throw new ArgumentNullException("attributeType");
			this.attributeType = attributeType;
			this.region = region;
			this.positionalArguments = positionalArguments ?? EmptyList<IConstantValue>.Instance;
			this.namedCtorArguments = namedCtorArguments ?? EmptyList<KeyValuePair<string, IConstantValue>>.Instance;
			this.namedArguments = namedArguments ?? EmptyList<KeyValuePair<string, IConstantValue>>.Instance;
		}
		
		public DomRegion Region {
			get { return region; }
		}
		
		public ITypeReference AttributeType {
			get { return attributeType; }
		}
		
		public IAttribute CreateResolvedAttribute(ITypeResolveContext context)
		{
			return new CSharpResolvedAttribute((VSharpTypeResolveContext)context, this);
		}
		
		sealed class CSharpResolvedAttribute : IAttribute
		{
			readonly VSharpTypeResolveContext context;
			readonly VSharpAttribute unresolved;
			readonly IType attributeType;
			
			IList<KeyValuePair<IMember, ResolveResult>> namedArguments;
			
			public CSharpResolvedAttribute(VSharpTypeResolveContext context, VSharpAttribute unresolved)
			{
				this.context = context;
				this.unresolved = unresolved;
				// Pretty much any access to the attribute checks the type first, so
				// we don't need to use lazy-loading for that.
				this.attributeType = unresolved.AttributeType.Resolve(context);
			}
			
			DomRegion IAttribute.Region {
				get { return unresolved.Region; }
			}
			
			IType IAttribute.AttributeType {
				get { return attributeType; }
			}
			
			ResolveResult ctorInvocation;
			
			InvocationResolveResult GetCtorInvocation()
			{
				ResolveResult rr = LazyInit.VolatileRead(ref this.ctorInvocation);
				if (rr != null) {
					return rr as InvocationResolveResult;
				} else {
					VSharpResolver resolver = new VSharpResolver(context);
					int totalArgumentCount = unresolved.positionalArguments.Count + unresolved.namedCtorArguments.Count;
					ResolveResult[] arguments = new ResolveResult[totalArgumentCount];
					string[] argumentNames = new string[totalArgumentCount];
					int i = 0;
					while (i < unresolved.positionalArguments.Count) {
						IConstantValue cv = unresolved.positionalArguments[i];
						arguments[i] = cv.Resolve(context);
						i++;
					}
					foreach (var pair in unresolved.namedCtorArguments) {
						argumentNames[i] = pair.Key;
						arguments[i] = pair.Value.Resolve(context);
						i++;
					}
					rr = resolver.ResolveObjectCreation(attributeType, arguments, argumentNames);
					return LazyInit.GetOrSet(ref this.ctorInvocation, rr) as InvocationResolveResult;
				}
			}
			
			IMethod IAttribute.Constructor {
				get {
					var invocation = GetCtorInvocation();
					if (invocation != null)
						return invocation.Member as IMethod;
					else
						return null;
				}
			}
			
			IList<ResolveResult> positionalArguments;
			
			IList<ResolveResult> IAttribute.PositionalArguments {
				get {
					var result = LazyInit.VolatileRead(ref this.positionalArguments);
					if (result != null) {
						return result;
					} else {
						var invocation = GetCtorInvocation();
						if (invocation != null)
							result = invocation.GetArgumentsForCall();
						else
							result = EmptyList<ResolveResult>.Instance;
						return LazyInit.GetOrSet(ref this.positionalArguments, result);
					}
				}
			}
			
			IList<KeyValuePair<IMember, ResolveResult>> IAttribute.NamedArguments {
				get {
					var namedArgs = LazyInit.VolatileRead(ref this.namedArguments);
					if (namedArgs != null) {
						return namedArgs;
					} else {
						namedArgs = new List<KeyValuePair<IMember, ResolveResult>>();
						foreach (var pair in unresolved.namedArguments) {
							IMember member = attributeType.GetMembers(m => (m.SymbolKind == SymbolKind.Field || m.SymbolKind == SymbolKind.Property) && m.Name == pair.Key).FirstOrDefault();
							if (member != null) {
								ResolveResult val = pair.Value.Resolve(context);
								namedArgs.Add(new KeyValuePair<IMember, ResolveResult>(member, val));
							}
						}
						return LazyInit.GetOrSet(ref this.namedArguments, namedArgs);
					}
				}
			}
		}
	}
}