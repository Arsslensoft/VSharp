using System;
using System.Collections.Generic;
using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
	public class ResolvedFieldSpec : ResolvedMemberSpec, IField
	{
		volatile ResolveResult constantValue;
		
		public ResolvedFieldSpec(IUnresolvedField unresolved, ITypeResolveContext parentContext)
			: base(unresolved, parentContext)
		{
		}
		
		public bool IsReadOnly {
			get { return ((IUnresolvedField)unresolved).IsReadOnly; }
		}
		
		public bool IsVolatile {
			get { return ((IUnresolvedField)unresolved).IsVolatile; }
		}
		
		IType IVariable.Type {
			get { return this.ReturnType; }
		}
		
		public bool IsConst {
			get { return ((IUnresolvedField)unresolved).IsConst; }
		}

		public bool IsFixed {
			get { return ((IUnresolvedField)unresolved).IsFixed; }
		}

		public object ConstantValue {
			get {
				ResolveResult rr = this.constantValue;
				if (rr == null) {
					using (var busyLock = BusyManager.Enter(this)) {
						if (!busyLock.Success)
							return null;

						IConstantValue unresolvedCV = ((IUnresolvedField)unresolved).ConstantValue;
                        if (unresolvedCV != null)
                            rr = unresolvedCV.Resolve(context);
                        else
                            //rr = ErrorResolveResult.UnknownError; //TODO:Fix this
                            rr = null;
						this.constantValue = rr;
					}
				}
				return rr.ConstantValue;
			}
		}
		
		public override IMember Specialize(TypeParameterSubstitution substitution)
		{
			if (TypeParameterSubstitution.Identity.Equals(substitution)
			    || DeclaringTypeDefinition == null
			    || DeclaringTypeDefinition.TypeParameterCount == 0)
			{
				return this;
			}
			if (substitution.MethodTypeArguments != null && substitution.MethodTypeArguments.Count > 0)
				substitution = new TypeParameterSubstitution(substitution.ClassTypeArguments, EmptyList<IType>.Instance);
			return new SpecializedField(this, substitution);
		}
		
		IMemberReference IField.ToReference()
		{
			return (IMemberReference)ToReference();
		}
	}
}
