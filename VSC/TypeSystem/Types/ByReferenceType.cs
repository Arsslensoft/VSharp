using System;
using VSC.TypeSystem.Implementation;

namespace VSC.TypeSystem
{
	public sealed class ByReferenceType : ElementTypeSpec
	{
		public ByReferenceType(IType elementType) : base(elementType)
		{
		}
		
		public override TypeKind Kind {
			get { return TypeKind.ByReference; }
		}
		
		public override string NameSuffix {
			get {
				return "&";
			}
		}
		
		public override bool? IsReferenceType {
			get { return null; }
		}
		
		public override int GetHashCode()
		{
			return elementType.GetHashCode() ^ 91725813;
		}
		
		public override bool Equals(IType other)
		{
			ByReferenceType a = other as ByReferenceType;
			return a != null && elementType.Equals(a.elementType);
		}
		
		public override IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitByReferenceType(this);
		}
		
		public override IType VisitChildren(TypeVisitor visitor)
		{
			IType e = elementType.AcceptVisitor(visitor);
			if (e == elementType)
				return this;
			else
				return new ByReferenceType(e);
		}
		
		public override ITypeReference ToTypeReference()
		{
			return new ByReferenceTypeReference(elementType.ToTypeReference());
		}
	}
	
	[Serializable]
	public sealed class ByReferenceTypeReference : ITypeReference, ISupportsInterning
	{
		readonly ITypeReference elementType;
		
		public ByReferenceTypeReference(ITypeReference elementType)
		{
			if (elementType == null)
				throw new ArgumentNullException("elementType");
			this.elementType = elementType;
		}
		
		public ITypeReference ElementType {
			get { return elementType; }
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			return new ByReferenceType(elementType.Resolve(context));
		}
		
		public override string ToString()
		{
			return elementType.ToString() + "&";
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			return elementType.GetHashCode() ^ 91725814;
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			ByReferenceTypeReference brt = other as ByReferenceTypeReference;
			return brt != null && this.elementType == brt.elementType;
		}
	}
}
