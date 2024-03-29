using System;
using System.Collections.Generic;
using System.Linq;
using VSC.TypeSystem.Implementation;
using VSC.Base;

namespace VSC.TypeSystem
{
	[Serializable]
    public sealed class MethodTypeParameterWithInheritedConstraints : UnresolvedTypeParameterSpec
    {
        public MethodTypeParameterWithInheritedConstraints(int index, string name,Location loc)
            : base(SymbolKind.Method, index,loc, name)
        {
        }

        static ITypeParameter ResolveBaseTypeParameter(IMethod parentMethod, int index)
        {
            IMethod baseMethod = null;
            if (parentMethod.IsOverride)
            {
                foreach (IMethod m in InheritanceHelper.GetBaseMembers(parentMethod, false).OfType<IMethod>())
                {
                    if (!m.IsOverride)
                    {
                        baseMethod = m;
                        break;
                    }
                }
            }
            else if (parentMethod.IsExplicitInterfaceImplementation && parentMethod.ImplementedInterfaceMembers.Count == 1)
            {
                baseMethod = parentMethod.ImplementedInterfaceMembers[0] as IMethod;
            }
            if (baseMethod != null && index < baseMethod.TypeParameters.Count)
                return baseMethod.TypeParameters[index];
            else
                return null;
        }

        public override ITypeParameter CreateResolvedTypeParameter(ITypeResolveContext context)
        {
            if (context.CurrentMember is IMethod)
            {
                return new ResolvedMethodTypeParameterWithInheritedConstraints(this, context);
            }
            else
            {
                return base.CreateResolvedTypeParameter(context);
            }
        }

        sealed class ResolvedMethodTypeParameterWithInheritedConstraints : TypeParameterSpec
        {
            volatile ITypeParameter baseTypeParameter;

            public ResolvedMethodTypeParameterWithInheritedConstraints(MethodTypeParameterWithInheritedConstraints unresolved, ITypeResolveContext context)
                : base(context.CurrentMember, unresolved.Index, unresolved.Name,  unresolved.Region)
            {
            }

            ITypeParameter GetBaseTypeParameter()
            {
                ITypeParameter baseTP = this.baseTypeParameter;
                if (baseTP == null)
                {
                    // ResolveBaseTypeParameter() is idempotent, so this is thread-safe.
                    this.baseTypeParameter = baseTP = ResolveBaseTypeParameter((IMethod)this.Owner, this.Index);
                }
                return baseTP;
            }

            public override bool HasValueTypeConstraint
            {
                get
                {
                    ITypeParameter baseTP = GetBaseTypeParameter();
                    return baseTP != null ? baseTP.HasValueTypeConstraint : false;
                }
            }

            public override bool HasReferenceTypeConstraint
            {
                get
                {
                    ITypeParameter baseTP = GetBaseTypeParameter();
                    return baseTP != null ? baseTP.HasReferenceTypeConstraint : false;
                }
            }

            public override bool HasDefaultConstructorConstraint
            {
                get
                {
                    ITypeParameter baseTP = GetBaseTypeParameter();
                    return baseTP != null ? baseTP.HasDefaultConstructorConstraint : false;
                }
            }

            public override IEnumerable<IType> DirectBaseTypes
            {
                get
                {
                    ITypeParameter baseTP = GetBaseTypeParameter();
                    if (baseTP != null)
                    {
                        // Substitute occurrences of the base method's type parameters in the constraints
                        // with the type parameters from the
                        IMethod owner = (IMethod)this.Owner;
                        var substitution = new TypeParameterSubstitution(null, new ProjectedList<ITypeParameter, IType>(owner.TypeParameters, t => t));
                        return baseTP.DirectBaseTypes.Select(t => t.AcceptVisitor(substitution));
                    }
                    else
                    {
                        return EmptyList<IType>.Instance;
                    }
                }
            }
        }
    }
}
