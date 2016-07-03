using System;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Base class for the visitor pattern on <see cref="IType"/>.
	/// </summary>
	public abstract class TypeVisitor
	{
		public virtual IType VisitTypeDefinition(ITypeDefinition type)
		{
			return type.VisitChildren(this);
		}
		
		public virtual IType VisitTypeParameter(ITypeParameter type)
		{
			return type.VisitChildren(this);
		}
		
		public virtual IType VisitParameterizedType(ParameterizedTypeSpec type)
		{
			return type.VisitChildren(this);
		}
		
		public virtual IType VisitArrayType(ArrayType type)
		{
			return type.VisitChildren(this);
		}
		
		public virtual IType VisitPointerType(PointerTypeSpec type)
		{
			return type.VisitChildren(this);
		}
		
		public virtual IType VisitByReferenceType(ByReferenceType type)
		{
			return type.VisitChildren(this);
		}
		
		public virtual IType VisitOtherType(IType type)
		{
			return type.VisitChildren(this);
		}
	}
}
