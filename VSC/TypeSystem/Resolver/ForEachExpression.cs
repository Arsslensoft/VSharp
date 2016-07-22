using System;
using VSC.AST;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// ResolveScope result representing a 'foreach' loop.
	/// </summary>
	public class ForEachExpression : Expression
	{
		/// <summary>
		/// Gets the semantic tree for the call to GetEnumerator.
		/// </summary>
		public readonly Expression GetEnumeratorCall;
		
		/// <summary>
		/// Gets the collection type.
		/// </summary>
		public readonly IType CollectionType;
		
		/// <summary>
		/// Gets the enumerator type.
		/// </summary>
		public readonly IType EnumeratorType;
		
		/// <summary>
		/// Gets the element type.
		/// This is the type that would be inferred for an implicitly-typed element variable.
		/// For explicitly-typed element variables, this type may differ from <c>ElementVariable.Type</c>.
		/// </summary>
		public readonly IType ElementType;
		
		/// <summary>
		/// Gets the element variable.
		/// </summary>
		public readonly IVariable ElementVariable;
		
		/// <summary>
		/// Gets the Current property on the IEnumerator.
		/// Returns null if the property is not found.
		/// </summary>
		public readonly IProperty CurrentProperty;
		
		/// <summary>
		/// Gets the MoveNext() method on the IEnumerator.
		/// Returns null if the method is not found.
		/// </summary>
		public readonly IMethod MoveNextMethod;
		
		public ForEachExpression(Expression getEnumeratorCall, IType collectionType, IType enumeratorType, IType elementType, IVariable elementVariable, IProperty currentProperty, IMethod moveNextMethod, IType voidType)
			: base(voidType)
		{
			if (getEnumeratorCall == null)
				throw new ArgumentNullException("getEnumeratorCall");
			if (collectionType == null)
				throw new ArgumentNullException("collectionType");
			if (enumeratorType == null)
				throw new ArgumentNullException("enumeratorType");
			if (elementType == null)
				throw new ArgumentNullException("elementType");
			if (elementVariable == null)
				throw new ArgumentNullException("elementVariable");
			this.GetEnumeratorCall = getEnumeratorCall;
			this.CollectionType = collectionType;
			this.EnumeratorType = enumeratorType;
			this.ElementType = elementType;
			this.ElementVariable = elementVariable;
			this.CurrentProperty = currentProperty;
			this.MoveNextMethod = moveNextMethod;
		}
	}
}
