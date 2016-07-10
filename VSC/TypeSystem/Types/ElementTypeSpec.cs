using System;

namespace VSC.TypeSystem.Implementation
{
	public abstract class ElementTypeSpec : TypeSpec, IEntity
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


        #region IEntity
        public DomRegion Region
        {
            get
            {
                if (elementType is IEntity) return (elementType as IEntity).Region;
                else return new DomRegion();
            }
        }

        public DomRegion BodyRegion
        {
            get { if (elementType is IEntity) return (elementType as IEntity).BodyRegion; else return new DomRegion(); }
        }

        public ITypeDefinition DeclaringTypeDefinition
        {
            get
            {
                if (elementType is IEntity)
                    return (elementType as IEntity).DeclaringTypeDefinition;
                else return null;
            }
        }

        public IAssembly ParentAssembly
        {
            get { if (elementType is IEntity) return (elementType as IEntity).ParentAssembly; else return null; }
        }

        System.Collections.Generic.IList<IAttribute> IEntity.Attributes
        {
            get { throw new NotImplementedException(); }
        }

        public ICompilation Compilation
        {
            get
            {
                if (elementType is IEntity)
                    return (elementType as IEntity).Compilation;
                else return null;
            }
        }

        public bool IsStatic
        {
            get { if (elementType is IEntity) return (elementType as IEntity).IsStatic; else return false; }
        }

        public bool IsAbstract
        {
            get { if (elementType is IEntity)return (elementType as IEntity).IsAbstract; else return false; }
        }

        public bool IsSealed
        {
            get { if (elementType is IEntity) return (elementType as IEntity).IsSealed; else return false; }
        }

        public bool IsShadowing
        {
            get { if (elementType is IEntity) return (elementType as IEntity).IsShadowing; else return false; }
        }

        public bool IsSynthetic
        {
            get { if (elementType is IEntity)return (elementType as IEntity).IsSynthetic; else return false; }
        }

        public bool IsBaseTypeDefinition(IType baseType)
        {
            if (elementType is IEntity) return (elementType as IEntity).IsBaseTypeDefinition(baseType); else return false;
        }

        public bool IsAccessibleAs(IType b)
        {
            if (elementType is IEntity) return (elementType as IEntity).IsAccessibleAs(b); else return false;
        }

        public bool IsInternalAccessible(IAssembly asm)
        {
            if (elementType is IEntity) return (elementType as IEntity).IsInternalAccessible(asm); else return false;
        }

        public SymbolKind SymbolKind
        {
            get { if (elementType is IEntity) return (elementType as IEntity).SymbolKind; else return SymbolKind.None; }
        }

        public ISymbolReference ToReference()
        {
            if (elementType is IEntity) return (elementType as IEntity).ToReference();
            else return null;
        }

        public Accessibility Accessibility
        {
            get { if (elementType is IEntity)return (elementType as IEntity).Accessibility; else return Accessibility.None; }
        }

        public bool IsPrivate
        {
            get { if (elementType is IEntity) return (elementType as IEntity).IsPrivate; else return false; }
        }

        public bool IsPublic
        {
            get { if (elementType is IEntity)return (elementType as IEntity).IsPublic; else return false; }
        }

        public bool IsProtected
        {
            get { if (elementType is IEntity)return (elementType as IEntity).IsProtected; else return false; }
        }

        public bool IsInternal
        {
            get
            {
                if (elementType is IEntity)return (elementType as IEntity).IsInternal;
                else return false;
                
            }
        }

        public bool IsProtectedOrInternal
        {
            get { if (elementType is IEntity)return (elementType as IEntity).IsProtectedOrInternal; else return false; }
        }

        #endregion


      
    }
}
