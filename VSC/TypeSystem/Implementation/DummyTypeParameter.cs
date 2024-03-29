using System;
using System.Collections.Generic;
using System.Threading;
using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
    public sealed class DummyTypeParameter : TypeSpec, ITypeParameter
    {
        static ITypeParameter[] methodTypeParameters = { new DummyTypeParameter(SymbolKind.Method, 0) };
        static ITypeParameter[] classTypeParameters = { new DummyTypeParameter(SymbolKind.TypeDefinition, 0) };

        public static ITypeParameter GetMethodTypeParameter(int index)
        {
            return GetTypeParameter(ref methodTypeParameters, SymbolKind.Method, index);
        }

        public static ITypeParameter GetClassTypeParameter(int index)
        {
            return GetTypeParameter(ref classTypeParameters, SymbolKind.TypeDefinition, index);
        }

        static ITypeParameter GetTypeParameter(ref ITypeParameter[] typeParameters, SymbolKind symbolKind, int index)
        {
            ITypeParameter[] tps = typeParameters;
            while (index >= tps.Length)
            {
                // We don't have a normal type parameter for this index, so we need to extend our array.
                // Because the array can be used concurrently from multiple threads, we have to use
                // Interlocked.CompareExchange.
                ITypeParameter[] newTps = new ITypeParameter[index + 1];
                tps.CopyTo(newTps, 0);
                for (int i = tps.Length; i < newTps.Length; i++)
                {
                    newTps[i] = new DummyTypeParameter(symbolKind, i);
                }
                ITypeParameter[] oldTps = Interlocked.CompareExchange(ref typeParameters, newTps, tps);
                if (oldTps == tps)
                {
                    // exchange successful
                    tps = newTps;
                }
                else
                {
                    // exchange not successful
                    tps = oldTps;
                }
            }
            return tps[index];
        }

        sealed class NormalizeMethodTypeParametersVisitor : TypeVisitor
        {
            public override IType VisitTypeParameter(ITypeParameter type)
            {
                if (type.OwnerType == SymbolKind.Method)
                {
                    return DummyTypeParameter.GetMethodTypeParameter(type.Index);
                }
                else
                {
                    return base.VisitTypeParameter(type);
                }
            }
        }
        sealed class NormalizeClassTypeParametersVisitor : TypeVisitor
        {
            public override IType VisitTypeParameter(ITypeParameter type)
            {
                if (type.OwnerType == SymbolKind.TypeDefinition)
                {
                    return DummyTypeParameter.GetClassTypeParameter(type.Index);
                }
                else
                {
                    return base.VisitTypeParameter(type);
                }
            }
        }

        static readonly NormalizeMethodTypeParametersVisitor normalizeMethodTypeParameters = new NormalizeMethodTypeParametersVisitor();
        static readonly NormalizeClassTypeParametersVisitor normalizeClassTypeParameters = new NormalizeClassTypeParametersVisitor();

        /// <summary>
        /// Replaces all occurrences of method type parameters in the given type
        /// by normalized type parameters. This allows comparing parameter types from different
        /// generic methods.
        /// </summary>
        public static IType NormalizeMethodTypeParameters(IType type)
        {
            return type.AcceptVisitor(normalizeMethodTypeParameters);
        }

        /// <summary>
        /// Replaces all occurrences of class type parameters in the given type
        /// by normalized type parameters. This allows comparing parameter types from different
        /// generic methods.
        /// </summary>
        public static IType NormalizeClassTypeParameters(IType type)
        {
            return type.AcceptVisitor(normalizeClassTypeParameters);
        }

        /// <summary>
        /// Replaces all occurrences of class and method type parameters in the given type
        /// by normalized type parameters. This allows comparing parameter types from different
        /// generic methods.
        /// </summary>
        public static IType NormalizeAllTypeParameters(IType type)
        {
            return type.AcceptVisitor(normalizeClassTypeParameters).AcceptVisitor(normalizeMethodTypeParameters);
        }

        readonly SymbolKind ownerType;
        readonly int index;

        private DummyTypeParameter(SymbolKind ownerType, int index)
        {
            this.ownerType = ownerType;
            this.index = index;
        }

        SymbolKind ISymbol.SymbolKind
        {
            get { return SymbolKind.TypeParameter; }
        }

        public override string Name
        {
            get
            {
                return (ownerType == SymbolKind.Method ? "!!" : "!") + index;
            }
        }

        public override string ReflectionName
        {
            get
            {
                return (ownerType == SymbolKind.Method ? "``" : "`") + index;
            }
        }

        public override string ToString()
        {
            return ReflectionName + " (dummy)";
        }

        public override bool? IsReferenceType
        {
            get { return null; }
        }

        public override TypeKind Kind
        {
            get { return TypeKind.TypeParameter; }
        }

        public override ITypeReference ToTypeReference()
        {
            return TypeParameterReference.Create(ownerType, index);
        }

        public override IType AcceptVisitor(TypeVisitor visitor)
        {
            return visitor.VisitTypeParameter(this);
        }

        public int Index
        {
            get { return index; }
        }

    
        SymbolKind ITypeParameter.OwnerType
        {
            get { return ownerType; }
        }



        DomRegion ITypeParameter.Region
        {
            get { return DomRegion.Empty; }
        }

        IEntity ITypeParameter.Owner
        {
            get { return null; }
        }

        IType ITypeParameter.EffectiveBaseClass
        {
            get { return SpecialTypeSpec.UnknownType; }
        }

        ICollection<IType> ITypeParameter.EffectiveInterfaceSet
        {
            get { return EmptyList<IType>.Instance; }
        }

        bool ITypeParameter.HasDefaultConstructorConstraint
        {
            get { return false; }
        }

        bool ITypeParameter.HasReferenceTypeConstraint
        {
            get { return false; }
        }

        bool ITypeParameter.HasValueTypeConstraint
        {
            get { return false; }
        }

        public ISymbolReference ToReference()
        {
            return new TypeParameterReference(ownerType, index);
        }
    }
}
