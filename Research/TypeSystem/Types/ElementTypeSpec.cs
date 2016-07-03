using System;

namespace VSC.TypeSystem.Implementation
{
	public abstract class ElementTypeSpec : TypeSpec
	{
		[CLSCompliant(false)]
		protected IType elementType;
		
		protected ElementTypeSpec(IType elementType)
		{
			if (elementType == null)
				throw new ArgumentNullException("elementType");
			this.elementType = elementType;
		}
		
		public override string Name {
			get { return elementType.Name + NameSuffix; }
		}
		
		public override string Namespace {
			get { return elementType.Namespace; }
		}
		
		public override string FullName {
			get { return elementType.FullName + NameSuffix; }
		}
		
		public override string ReflectionName {
			get { return elementType.ReflectionName + NameSuffix; }
		}
		
		public abstract string NameSuffix { get; }
		
		public IType ElementType {
			get { return elementType; }
		}
		
		// Force concrete implementations to override VisitChildren - the base implementation
		// in AbstractType assumes there are no children, but we know there is (at least) 1.
		public abstract override IType VisitChildren(TypeVisitor visitor);
	}
}
