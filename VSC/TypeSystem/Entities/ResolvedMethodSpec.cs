using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSC.Base;


namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation of <see cref="IMethod"/> that resolves an unresolved method.
	/// </summary>
	public class ResolvedMethodSpec : ResolvedMemberSpec, IMethod
	{
		IUnresolvedMethod[] parts;
		
		public ResolvedMethodSpec(UnresolvedMethodSpec unresolved, ITypeResolveContext parentContext)
			: this(unresolved, parentContext, unresolved.IsExtensionMethod)
		{
		}
		
		public ResolvedMethodSpec(IUnresolvedMethod unresolved, ITypeResolveContext parentContext, bool isExtensionMethod)
			: base(unresolved, parentContext)
		{
			this.Parameters = unresolved.Parameters.CreateResolvedParameters(context);
			this.ReturnTypeAttributes = unresolved.ReturnTypeAttributes.CreateResolvedAttributes(parentContext);
			this.TypeParameters = unresolved.TypeParameters.CreateResolvedTypeParameters(context);
			this.IsExtensionMethod = isExtensionMethod;
		}

		class ListOfLists<T> : IList<T>
		{
			List<IList<T>> lists =new List<IList<T>> ();

			public void AddList(IList<T> list)
			{
				lists.Add (list);
			}

			#region IEnumerable implementation
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
			#endregion

			#region IEnumerable implementation
			public IEnumerator<T> GetEnumerator ()
			{
				for (int i = 0; i < this.Count; i++) {
					yield return this[i];
				}
			}
			#endregion

			#region ICollection implementation
			public void Add (T item)
			{
				throw new NotSupportedException();
			}

			public void Clear ()
			{
				throw new NotSupportedException();
			}

			public bool Contains (T item)
			{
				var comparer = EqualityComparer<T>.Default;
				for (int i = 0; i < this.Count; i++) {
					if (comparer.Equals(this[i], item))
						return true;
				}
				return false;
			}

			public void CopyTo (T[] array, int arrayIndex)
			{
				for (int i = 0; i < Count; i++) {
					array[arrayIndex + i] = this[i];
				}
			}

			public bool Remove (T item)
			{
				throw new NotSupportedException();
			}

			public int Count {
				get {
					return lists.Sum (l => l.Count);
				}
			}

			public bool IsReadOnly {
				get {
					return true;
				}
			}
			#endregion

			#region IList implementation
			public int IndexOf (T item)
			{
				var comparer = EqualityComparer<T>.Default;
				for (int i = 0; i < this.Count; i++) {
					if (comparer.Equals(this[i], item))
						return i;
				}
				return -1;
			}

			public void Insert (int index, T item)
			{
				throw new NotSupportedException();
			}

			public void RemoveAt (int index)
			{
				throw new NotSupportedException();
			}

			public T this[int index] {
				get {
					foreach (var list in lists){
						if (index < list.Count)
							return list[index];
						index -= list.Count;
					}
					throw new IndexOutOfRangeException ();
				}
				set {
					throw new NotSupportedException();
				}
			}
			#endregion
		}

		public static ResolvedMethodSpec CreateFromMultipleParts(IUnresolvedMethod[] parts, ITypeResolveContext[] contexts, bool isExtensionMethod)
		{
			ResolvedMethodSpec method = new ResolvedMethodSpec(parts[0], contexts[0], isExtensionMethod);
			method.parts = parts;
			if (parts.Length > 1) {
				var attrs = new ListOfLists <IAttribute>();
				attrs.AddList (method.Attributes);
				for (int i = 1; i < parts.Length; i++) {
					attrs.AddList (parts[i].Attributes.CreateResolvedAttributes(contexts[i]));
				}
				method.Attributes = attrs;
			}
			return method;
		}
		
		public IList<IParameter> Parameters { get; private set; }
		public IList<IAttribute> ReturnTypeAttributes { get; private set; }
		public IList<ITypeParameter> TypeParameters { get; private set; }

		public IList<IType> TypeArguments {
			get {
				// ToList() call is necessary because IList<> isn't covariant
				return TypeParameters.ToList<IType>();
			}
		}
		
		bool IMethod.IsParameterized {
			get { return false; }
		}

		public bool IsExtensionMethod { get; private set; }
		
		public IList<IUnresolvedMethod> Parts {
			get {
				return parts ?? new IUnresolvedMethod[] { (IUnresolvedMethod)unresolved };
			}
		}
        public bool IsSupersede
        {
            get { return ((IUnresolvedMethod)unresolved).IsSupersede; }
        }
		public bool IsConstructor {
			get { return ((IUnresolvedMethod)unresolved).IsConstructor; }
		}
		
		public bool IsDestructor {
			get { return ((IUnresolvedMethod)unresolved).IsDestructor; }
		}
		
		public bool IsOperator {
			get { return ((IUnresolvedMethod)unresolved).IsOperator; }
		}
	
		public bool IsAsync {
			get { return ((IUnresolvedMethod)unresolved).IsAsync; }
		}

		public bool HasBody {
			get { return ((IUnresolvedMethod)unresolved).HasBody; }
		}
		
		public bool IsAccessor {
			get { return ((IUnresolvedMethod)unresolved).AccessorOwner != null; }
		}

		IMethod IMethod.ReducedFrom {
			get { return null; }
		}

		public virtual IMember AccessorOwner {
			get {
				var reference = ((IUnresolvedMethod)unresolved).AccessorOwner;
				if (reference != null)
					return reference.Resolve(context);
				else
					return null;
			}
		}
		
		public override ISymbolReference ToReference()
		{
			var declType = this.DeclaringType;
			var declTypeRef = declType != null ? declType.ToTypeReference() : SpecialTypeSpec.UnknownType;
			if (IsExplicitInterfaceImplementation && ImplementedInterfaceMembers.Count == 1) {
				return new ExplicitInterfaceImplementationMemberReference(declTypeRef, ImplementedInterfaceMembers[0].ToReference());
			} else {
				return new MemberReferenceSpec(
					this.SymbolKind, declTypeRef, this.Name, this.TypeParameters.Count,
					this.Parameters.Select(p => p.Type.ToTypeReference()).ToList());
			}
		}
		
		public override IMemberReference ToMemberReference()
		{
			return (IMemberReference)ToReference();
		}
		
		public override IMember Specialize(TypeParameterSubstitution substitution)
		{
			if (TypeParameterSubstitution.Identity.Equals(substitution))
				return this;
			if (TypeParameters.Count == 0) {
				if (DeclaringTypeDefinition == null || DeclaringTypeDefinition.TypeParameterCount == 0)
					return this;
				if (substitution.MethodTypeArguments != null && substitution.MethodTypeArguments.Count > 0)
					substitution = new TypeParameterSubstitution(substitution.ClassTypeArguments, EmptyList<IType>.Instance);
			}
			return new SpecializedMethod(this, substitution);
		}
		
		IMethod IMethod.Specialize(TypeParameterSubstitution substitution)
		{
			return (IMethod)Specialize(substitution);
		}
		
		public override string ToString()
		{
			StringBuilder b = new StringBuilder("[");
			b.Append(this.SymbolKind);
			b.Append(' ');
			if (this.DeclaringType != null) {
				b.Append(this.DeclaringType.ReflectionName);
				b.Append('.');
			}
			b.Append(this.Name);
			if (this.TypeParameters.Count > 0) {
				b.Append("``");
				b.Append(this.TypeParameters.Count);
			}
			b.Append('(');
			for (int i = 0; i < this.Parameters.Count; i++) {
				if (i > 0) b.Append(", ");
				b.Append(this.Parameters[i].ToString());
			}
			b.Append("):");
			b.Append(this.ReturnType.ReflectionName);
			b.Append(']');
			return b.ToString();
		}
		
		/// <summary>
		/// Gets a dummy constructor for the specified compilation.
		/// </summary>
		/// <returns>
		/// A public instance constructor with IsSynthetic=true and no declaring type.
		/// </returns>
		/// <seealso cref="UnresolvedMethodSpec.DummyConstructor"/>
		public static IMethod GetDummyConstructor(ICompilation compilation)
		{
			var dummyConstructor = UnresolvedMethodSpec.DummyConstructor;
			// Reuse the same IMethod instance for all dummy constructors
			// so that two occurrences of 'new T()' refer to the same constructor.
			return (IMethod)compilation.CacheManager.GetOrAddShared(
				dummyConstructor, _ => dummyConstructor.CreateResolved(compilation.TypeResolveContext));
		}
		
		/// <summary>
		/// Gets a dummy constructor for the specified type.
		/// </summary>
		/// <returns>
		/// A public instance constructor with IsSynthetic=true and the specified declaring type.
		/// </returns>
		/// <seealso cref="UnresolvedMethodSpec.DummyConstructor"/>
		public static IMethod GetDummyConstructor(ICompilation compilation, IType declaringType)
		{
			var resolvedCtor = GetDummyConstructor(compilation);
			return new SpecializedMethod(resolvedCtor, TypeParameterSubstitution.Identity) { DeclaringType = declaringType };
		}
	}
}
