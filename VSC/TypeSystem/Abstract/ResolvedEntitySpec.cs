using System;
using System.Collections.Generic;
using System.Linq;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Implementation of <see cref="IEntity"/> that resolves an unresolved entity.
	/// </summary>
	public abstract class ResolvedEntitySpec : IEntity
	{
		protected readonly IUnresolvedEntity unresolved;
		protected readonly ITypeResolveContext parentContext;
		
		protected ResolvedEntitySpec(IUnresolvedEntity unresolved, ITypeResolveContext parentContext)
		{
			if (unresolved == null)
				throw new ArgumentNullException("unresolved");
			if (parentContext == null)
				throw new ArgumentNullException("parentContext");
			this.unresolved = unresolved;
			this.parentContext = parentContext;
			this.Attributes = unresolved.Attributes.CreateResolvedAttributes(parentContext);
		}
		
		public SymbolKind SymbolKind {
			get { return unresolved.SymbolKind; }
		}
		

		public DomRegion Region {
			get { return unresolved.Region; }
		}
		
		public DomRegion BodyRegion {
			get { return unresolved.BodyRegion; }
		}
		
		public ITypeDefinition DeclaringTypeDefinition {
			get { return parentContext.CurrentTypeDefinition; }
		}
		
		public virtual IType DeclaringType {
			get { return parentContext.CurrentTypeDefinition; }
		}
		
		public IAssembly ParentAssembly {
			get { return parentContext.CurrentAssembly; }
		}
		
		public IList<IAttribute> Attributes { get; protected set; }
		
	
		public abstract ISymbolReference ToReference();
		
		public bool IsStatic { get { return unresolved.IsStatic; } }
		public bool IsAbstract { get { return unresolved.IsAbstract; } }
		public bool IsSealed { get { return unresolved.IsSealed; } }
		public bool IsShadowing { get { return unresolved.IsShadowing; } }
		public bool IsSynthetic { get { return unresolved.IsSynthetic; } }
		
		public ICompilation Compilation {
			get { return parentContext.Compilation; }
		}
		
		public string FullName { get { return unresolved.FullName; } }
		public string Name { get { return unresolved.Name; } }
		public string ReflectionName { get { return unresolved.ReflectionName; } }
		public string Namespace { get { return unresolved.Namespace; } }
		
		public virtual Accessibility Accessibility { get { return unresolved.Accessibility; } }
		public bool IsPrivate { get { return Accessibility == Accessibility.Private; } }
		public bool IsPublic { get { return Accessibility == Accessibility.Public; } }
		public bool IsProtected { get { return Accessibility == Accessibility.Protected; } }
		public bool IsInternal { get { return Accessibility == Accessibility.Internal; } }
		public bool IsProtectedOrInternal { get { return Accessibility == Accessibility.ProtectedOrInternal; } }

		
		public override string ToString()
		{
			return "[" + this.SymbolKind.ToString() + " " + this.ReflectionName + "]";
		}

        public bool IsAccessibleAs(IType b)
        {
            //
            // if M is private, its accessibility is the same as this declspace.
            // we already know that P is accessible to T before this method, so we
            // may return true.
            //

            if (Accessibility == Accessibility.Private)
                return true;

            while (b is ElementTypeSpec)
                b = (b as ElementTypeSpec).ElementType;

            if (b is TypeParameterSpec)
                return true;

            for (IType p_parent; b != null; b = p_parent)
            {
                p_parent = b.DeclaringType;

                if (b.IsParameterized)
                {
                    foreach (var t in b.TypeArguments)
                    {
                        if (!IsAccessibleAs(t))
                            return false;
                    }
                }

                var pAccess = (b as IEntity).Accessibility;
                if (pAccess == Accessibility.Public)
                    continue;
                bool same_access_restrictions = false;
                for (IType mc = this as IType; !same_access_restrictions && mc != null && mc.DirectBaseTypes.FirstOrDefault() != null; mc = mc.DirectBaseTypes.FirstOrDefault())
                {
                    var al = (mc as IEntity).Accessibility;
                    switch (pAccess)
                    {
                        case Accessibility.Internal:
                            if (al == Accessibility.Private || al == Accessibility.Internal)
                                same_access_restrictions = IsInternalAccessible((b as IEntity).ParentAssembly, (mc as IEntity).ParentAssembly);
                            break;

                        case Accessibility.Protected:
                            if (al == Accessibility.Protected)
                            {
                                same_access_restrictions =
                                    (mc.DirectBaseTypes.FirstOrDefault() as IEntity).IsBaseTypeDefinition(p_parent);
                                break;
                            }

                            if (al == Accessibility.Private)
                            {
                                //
                                // When type is private and any of its parents derives from
                                // protected type then the type is accessible
                                //
                                while (mc.DirectBaseTypes.FirstOrDefault() != null)
                                {
                                    if ((mc.DirectBaseTypes.FirstOrDefault() as IEntity).IsBaseTypeDefinition(p_parent))
                                    {
                                        same_access_restrictions = true;
                                        break;
                                    }

                                    mc = mc.DirectBaseTypes.FirstOrDefault();
                                }
                            }

                            break;

                        case Accessibility.ProtectedOrInternal:
                            if (al == Accessibility.Internal)
                                same_access_restrictions = IsInternalAccessible((b as IEntity).ParentAssembly, (mc as IEntity).ParentAssembly);
                            else if (al == Accessibility.ProtectedOrInternal)
                                same_access_restrictions = (mc.DirectBaseTypes.FirstOrDefault() as IEntity).IsBaseTypeDefinition(p_parent) && IsInternalAccessible((b as IEntity).ParentAssembly,(mc as IEntity).ParentAssembly);
                            else if (al == Accessibility.Protected)
                                goto case TypeSystem.Accessibility.Protected;
                            else if (al == Accessibility.Private)
                            {
                                if (IsInternalAccessible((b as IEntity).ParentAssembly, (mc as IEntity).ParentAssembly))
                                {
                                    same_access_restrictions = true;
                                }
                                else
                                {
                                    goto case TypeSystem.Accessibility.Protected;
                                }
                            }

                            break;

                        case Accessibility.Private:
                            //
                            // Both are private and share same parent
                            //
                            if (al == Accessibility.Private)
                            {
                                var decl = mc.DirectBaseTypes.FirstOrDefault();
                                do
                                {
                                    same_access_restrictions = decl == p_parent;
                                } while (!same_access_restrictions && decl.DeclaringType == null && (decl = decl.DirectBaseTypes.FirstOrDefault()) != null);
                            }

                            break;

                        default:
                            throw new InternalErrorException(al.ToString());
                    }
                }

                if (!same_access_restrictions)
                    return false;

            }

            return true;
        }
        //
        // Used for visiblity checks to tests whether this definition shares
        // base type baseType, it does member-definition search
        //
        public bool IsBaseTypeDefinition(IType baseType)
        {
            var type = this as IType;
            if (baseType == null || type == null)
                return false;

            foreach(var t in type.DirectBaseTypes.Cast<ResolvedEntitySpec>())
                if (t.IsBaseTypeDefinition(baseType))
                    return true;




            return type == baseType;
        }
        public bool IsInternalAccessible(IAssembly asm, IAssembly current)
	    {
            return asm != null && current != null && asm.InternalsVisibleTo(current);
	    }
	}
}
