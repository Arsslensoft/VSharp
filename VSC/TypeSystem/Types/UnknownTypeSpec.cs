using System;
using System.Diagnostics;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// An unknown type where (part) of the name is known.
	/// </summary>
	[Serializable]
	public class UnknownTypeSpec : TypeSpec, ITypeReference
	{
		readonly bool namespaceKnown;
		readonly FullTypeName fullTypeName;
		
		/// <summary>
		/// Creates a new unknown type.
		/// </summary>
		/// <param name="namespaceName">Namespace name, if known. Can be null if unknown.</param>
		/// <param name="name">Name of the type, must not be null.</param>
		/// <param name="typeParameterCount">Type parameter count, zero if unknown.</param>
		public UnknownTypeSpec(string namespaceName, string name, int typeParameterCount = 0)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			this.namespaceKnown = namespaceName != null;
			this.fullTypeName = new TopLevelTypeName(namespaceName ?? string.Empty, name, typeParameterCount);
		}
		
		/// <summary>
		/// Creates a new unknown type.
		/// </summary>
		/// <param name="fullTypeName">Full name of the unknown type.</param>
		public UnknownTypeSpec(FullTypeName fullTypeName)
		{
			if (fullTypeName.Name == null) {
				Debug.Assert(fullTypeName == default(FullTypeName));
				this.namespaceKnown = false;
				this.fullTypeName = new TopLevelTypeName(string.Empty, "?", 0);
			} else {
				this.namespaceKnown = true;
				this.fullTypeName = fullTypeName;
			}
		}
		
		public override TypeKind Kind {
			get { return TypeKind.Unknown; }
		}
		
		public override ITypeReference ToTypeReference()
		{
			return this;
		}
		
		IType ITypeReference.Resolve(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			return this;
		}
		
		public override string Name {
			get { return fullTypeName.Name; }
		}
		
		public override string Namespace {
			get { return fullTypeName.TopLevelTypeName.Namespace; }
		}
		
		public override string ReflectionName {
			get { return namespaceKnown ? fullTypeName.ReflectionName : "?"; }
		}
		
		public override int TypeParameterCount {
			get { return fullTypeName.TypeParameterCount; }
		}
		
		public override bool? IsReferenceType {
			get { return null; }
		}
		
		public override int GetHashCode()
		{
			return (namespaceKnown ? 812571 : 12651) ^ fullTypeName.GetHashCode();
		}
		
		public override bool Equals(IType other)
		{
			UnknownTypeSpec o = other as UnknownTypeSpec;
			if (o == null)
				return false;
			return this.namespaceKnown == o.namespaceKnown && this.fullTypeName == o.fullTypeName;
		}
		
		public override string ToString()
		{
			return "[UnknownTypeSpec " + fullTypeName.ReflectionName + "]";
		}
	}
}
