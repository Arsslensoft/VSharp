using System;
using System.Collections.Generic;
using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Special type definition for 'void'.
	/// </summary>
	public class VoidTypeDefinition : ResolvedTypeDefinitionSpec
	{
		public VoidTypeDefinition(ITypeResolveContext parentContext, params IUnresolvedTypeDefinition[] parts)
			: base(parentContext, parts)
		{
		}
		
		public override TypeKind Kind {
			get { return TypeKind.Void; }
		}
		
		public override IEnumerable<IMethod> GetConstructors(Predicate<IUnresolvedMethod> filter, GetMemberOptions options)
		{
			return EmptyList<IMethod>.Instance;
		}
		
		public override IEnumerable<IEvent> GetEvents(Predicate<IUnresolvedEvent> filter, GetMemberOptions options)
		{
			return EmptyList<IEvent>.Instance;
		}
		
		public override IEnumerable<IField> GetFields(Predicate<IUnresolvedField> filter, GetMemberOptions options)
		{
			return EmptyList<IField>.Instance;
		}
		
		public override IEnumerable<IMethod> GetMethods(Predicate<IUnresolvedMethod> filter, GetMemberOptions options)
		{
			return EmptyList<IMethod>.Instance;
		}
		
		public override IEnumerable<IMethod> GetMethods(IList<IType> typeArguments, Predicate<IUnresolvedMethod> filter, GetMemberOptions options)
		{
			return EmptyList<IMethod>.Instance;
		}
		
		public override IEnumerable<IProperty> GetProperties(Predicate<IUnresolvedProperty> filter, GetMemberOptions options)
		{
			return EmptyList<IProperty>.Instance;
		}
		
		public override IEnumerable<IMember> GetMembers(Predicate<IUnresolvedMember> filter, GetMemberOptions options)
		{
			return EmptyList<IMember>.Instance;
		}
	}
}