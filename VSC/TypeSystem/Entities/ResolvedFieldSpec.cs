using System;
using System.Collections.Generic;
using VSC.AST;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem.Resolver;

namespace VSC.TypeSystem.Implementation
{
	public class ResolvedFieldSpec : ResolvedMemberSpec, IField
	{
		public volatile AST.Expression constantValue;
		
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

	    public Location ConstantLocation;
		public object ConstantValue {
			get {
         
				Expression rr = this.constantValue;
			  
				if (rr == null) {
					using (var busyLock = BusyManager.Enter(this)) {
					    if (!busyLock.Success)
					    {
                            CompilerContext.report.Error(110, ConstantLocation,
                    "The evaluation of the constant value for `{0}' involves a circular definition",
                    ToString());
                            return null;
					    }
							

						IConstantValue unresolvedCV = ((IUnresolvedField)unresolved).ConstantValue;
                        if (unresolvedCV != null)
                            rr = unresolvedCV.ResolveConstant(context);
                        else
                            rr = ErrorExpression.UnknownError; //TODO:Fix this
                            //rr = null;
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
