using System;
using System.Collections.Generic;
using VSC.TypeSystem.Implementation;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Contains static implementations of special types.
	/// </summary>
	[Serializable]
	public sealed class SpecialTypeSpec : TypeSpec, ITypeReference
	{
		/// <summary>
		/// Gets the type representing resolve errors.
		/// </summary>
		public readonly static SpecialTypeSpec UnknownType = new SpecialTypeSpec(TypeKind.Unknown, "?", isReferenceType: null);
		
		/// <summary>
		/// The null type is used as type of the null literal. It is a reference type without any members; and it is a subtype of all reference types.
		/// </summary>
		public readonly static SpecialTypeSpec NullType = new SpecialTypeSpec(TypeKind.Null, "null", isReferenceType: true);
		
		/// <summary>
		/// Type representing the V# 'dynamic' type.
		/// </summary>
		public readonly static SpecialTypeSpec Dynamic = new SpecialTypeSpec(TypeKind.Dynamic, "dynamic", isReferenceType: true);
		
		/// <summary>
		/// Type representing the result of the V# '__arglist()' expression.
		/// </summary>
		public readonly static SpecialTypeSpec ArgList = new SpecialTypeSpec(TypeKind.ArgList, "__arglist", isReferenceType: null);

        /// <summary>
        /// Type representing the result of the V# 'fake' expression.
        /// </summary>
        public readonly static SpecialTypeSpec FakeType = new SpecialTypeSpec(TypeKind.Fake, "fake", isReferenceType: null);
        /// <summary>
        /// A type used for unbound type arguments in partially parameterized types.
        /// </summary>
        public readonly static SpecialTypeSpec UnboundTypeArgument = new SpecialTypeSpec(TypeKind.UnboundTypeArgument, "", isReferenceType: null);
		
		readonly TypeKind kind;
		readonly string name;
		readonly bool? isReferenceType;
		
		private SpecialTypeSpec(TypeKind kind, string name, bool? isReferenceType)
		{
			this.kind = kind;
			this.name = name;
			this.isReferenceType = isReferenceType;
		}
		
		public override ITypeReference ToTypeReference()
		{
			return this;
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override TypeKind Kind {
			get { return kind; }
		}
		
		public override bool? IsReferenceType {
			get { return isReferenceType; }
		}
		
		IType ITypeReference.Resolve(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			return this;
		}
		
		#pragma warning disable 809
		[Obsolete("Please compare special types using the kind property instead.")]
		public override bool Equals(IType other)
		{
			// We consider a special types equal when they have equal types.
			// However, an unknown type with additional information is not considered to be equal to the SpecialTypeSpec with TypeKind.Unknown.
			return other is SpecialTypeSpec && other.Kind == kind;
		}
		
		public override int GetHashCode()
		{
			return 81625621 ^ (int)kind;
		}
	}
}
