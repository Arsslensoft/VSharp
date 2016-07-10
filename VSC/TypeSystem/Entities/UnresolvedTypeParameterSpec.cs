using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VSC.AST;
using VSC.Base;
using VSC.TypeSystem.Resolver;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation of <see cref="IUnresolvedTypeParameter"/>.
	/// </summary>
	[Serializable]
	public class UnresolvedTypeParameterSpec  : IUnresolvedTypeParameter, IFreezable, IResolve
	{
		readonly int index;
		IList<ITypeReference> constraints;
		string name;
		DomRegion region;
	    public Location Location;
		SymbolKind ownerType;
		BitVector16 flags;
		const ushort FlagFrozen                       = 0x0001;
		const ushort FlagReferenceTypeConstraint      = 0x0002;
		const ushort FlagValueTypeConstraint          = 0x0004;
		const ushort FlagDefaultConstructorConstraint = 0x0008;
		
		public void Freeze()
		{
			if (!flags[FlagFrozen]) {
				FreezeInternal();
				flags[FlagFrozen] = true;
			}
		}
		
		protected virtual void FreezeInternal()
		{
			constraints = FreezableHelper.FreezeList(constraints);
		}

        public UnresolvedTypeParameterSpec(SymbolKind ownerType, int index, Location loc,string name = null)
        {
            Location = loc;
			this.ownerType = ownerType;
			this.index = index;
			this.name = name ?? ((ownerType == SymbolKind.Method ? "!!" : "!") + index.ToString(CultureInfo.InvariantCulture));
		}
		
		public SymbolKind OwnerType {
			get { return ownerType; }
            set { ownerType = value; }
		}
		
		public int Index {
			get { return index; }
		}
		
		public bool IsFrozen {
			get { return flags[FlagFrozen]; }
		}
		
		public string Name {
			get { return name; }
			set {
				FreezableHelper.ThrowIfFrozen(this);
				name = value;
			}
		}
		
		string INamedElement.FullName {
			get { return name; }
		}
		
		string INamedElement.Namespace {
			get { return string.Empty; }
		}
		
		string INamedElement.ReflectionName {
			get {
				if (ownerType == SymbolKind.Method)
					return "``" + index.ToString(CultureInfo.InvariantCulture);
				else
					return "`" + index.ToString(CultureInfo.InvariantCulture);
			}
		}
		
		
		public IList<ITypeReference> Constraints {
			get {
				if (constraints == null)
					constraints = new List<ITypeReference>();
				return constraints;
			}
		}
		
	
		public DomRegion Region {
			get { return region; }
			set {
				FreezableHelper.ThrowIfFrozen(this);
				region = value;
			}
		}
		
		public bool HasDefaultConstructorConstraint {
			get { return flags[FlagDefaultConstructorConstraint]; }
			set {
				FreezableHelper.ThrowIfFrozen(this);
				flags[FlagDefaultConstructorConstraint] = value;
			}
		}
		
		public bool HasReferenceTypeConstraint {
			get { return flags[FlagReferenceTypeConstraint]; }
			set {
				FreezableHelper.ThrowIfFrozen(this);
				flags[FlagReferenceTypeConstraint] = value;
			}
		}
		
		public bool HasValueTypeConstraint {
			get { return flags[FlagValueTypeConstraint]; }
			set {
				FreezableHelper.ThrowIfFrozen(this);
				flags[FlagValueTypeConstraint] = value;
			}
		}
		
		/// <summary>
		/// Uses the specified interning provider to intern
		/// strings and lists in this entity.
		/// This method does not test arbitrary objects to see if they implement ISupportsInterning;
		/// instead we assume that those are interned immediately when they are created (before they are added to this entity).
		/// </summary>
		public virtual void ApplyInterningProvider(InterningProvider provider)
		{
			if (provider == null)
				throw new ArgumentNullException("provider");
			FreezableHelper.ThrowIfFrozen(this);
			name = provider.Intern(name);
			constraints = provider.InternList(constraints);
		}
		
		public virtual ITypeParameter CreateResolvedTypeParameter(ITypeResolveContext context)
		{
			IEntity owner = null;
			if (this.OwnerType == SymbolKind.Method) {
				owner = context.CurrentMember as IMethod;
			} else if (this.OwnerType == SymbolKind.TypeDefinition) {
				owner = context.CurrentTypeDefinition;
			}
			if (owner == null)
				throw new InvalidOperationException("Could not determine the type parameter's owner.");
			return new ResolvedTypeParameterSpec(
				owner, index, name, this.Region,
				this.HasValueTypeConstraint, this.HasReferenceTypeConstraint, this.HasDefaultConstructorConstraint, this.Constraints.Resolve(context)
			);
		}





	    void CheckConversion(ResolveContext rc, IType tp)
	    {
	     
       

	    }
	    public ITypeParameter ResolvedTypeParameter; 
	    void CheckCircular(TypeParameterSpec p, TypeParameterSpec baseType, ResolveContext rc, bool ignoreFirst = false, TypeParameterSpec parent = null)
	    {
	        if (p == baseType && !ignoreFirst)
	        {
                rc.Report.Error(173, Location,
                  "Circular constraint dependency involving `{0}' and `{1}'",
                  p.Name, parent.Name);
                return;
	        }



	        foreach (var t in baseType.DirectBaseTypes.Where(x => x is TypeParameterSpec))
	            CheckCircular(p, t as TypeParameterSpec, rc, false, baseType);
	      
	    }
        public bool Resolve(Resolver.ResolveContext rc)
        {
            
            ResolvedTypeParameter = CreateResolvedTypeParameter(rc.CurrentTypeResolveContext);  
           
            // check for conflicting constraints (struct & class)
        //    if(ResolvedTypeParameter.HasReferenceTypeConstraint && ResolvedTypeParameter.HasValueTypeConstraint )
        //        rc.Report.Error(174, Location,
        //"Type parameter `{0}' inherits conflicting constraints `{1}' and `{2}'",
        //name, "struct","class");

            
                            
            // check direct base types
            List<IType> checked_types = new List<IType>();
            foreach (var type in ResolvedTypeParameter.DirectBaseTypes)
            {
                // check for duplicate constraints
                if (checked_types.Contains(type))
                {
                    rc.Report.Error(176, Location,
                            "Duplicate constraint `{0}' for type parameter `{1}'", type.ToString(), name);
                    continue;

                }
                checked_types.Add(type);
                // check accessibility of type & constraint
                if (!ResolvedTypeParameter.Owner.IsAccessibleAs(type))
                    rc.Report.Error(177, Location,
                             "Inconsistent accessibility: constraint type `{0}' is less accessible than `{1}'",
                             type.ToString(), ResolvedTypeParameter.Owner.ToString());


                if (type is TypeParameterSpec)
                {
                    TypeParameterSpec t = (type as TypeParameterSpec);
                    if (t.HasValueTypeConstraint)
                        rc.Report.Error(178, Location,
                            "Type parameter `{0}' has the `struct' constraint, so it cannot be used as a constraint for `{1}'",
                            t.Name, name);

                    // check for circular
                    CheckCircular(t, t, rc, true);


                    //
                    // Checks whether there are no conflicts between type parameter constraints
                    //
                    // class Foo<T, U>
                    //      where T : A
                    //      where U : B, T
                    //
                    // A and B are not convertible and only 1 class constraint is allowed
                    //TODO: Add Conversion check


                    // check for class type and (class or struct constraint)
                    if ((t.HasValueTypeConstraint && ResolvedTypeParameter.HasReferenceTypeConstraint) ||
                        (t.HasReferenceTypeConstraint && ResolvedTypeParameter.HasValueTypeConstraint))
                    {
                        rc.Report.Error(179, Location,
                            "`{0}' and `{1}' : cannot specify both `class' and `struct' constraint",
                            t.Name, Name);
                    }

                }
                else
                {

                    // check static or sealed
                    if ((type as IEntity).IsSealed)
                    {
                        rc.Report.Error(180, Location,
                            "`{0}' A constraint must be an interface,  a type parameter or a non sealed/static class",
                            type.ToString());
                    }
                }

                // primitive types check
                if (type.IsBuiltinType())
                 rc.Report.Error(182, Location,
                       "The type `{0}' cannot not be used as a base type for type parameter `{1}' in type or method `{2}'. ",
                    type.ToString(), name, ResolvedTypeParameter.Owner.ToString());



            }
            
            
            // constructor constraint
            if (ResolvedTypeParameter.HasDefaultConstructorConstraint && !ResolvedTypeParameter.EffectiveBaseClass.GetConstructors(x => x.Parameters.Count == 0).Any())
                rc.Report.Error(181, Location,
                        "The type `{0}' must have a public parameterless constructor in order to use it as parameter `{1}' in the generic type or method `{2}'",
                       ResolvedTypeParameter.EffectiveBaseClass.ToString(), name, ResolvedTypeParameter.Owner.ToString());

	        
            return true;
        }
    }



    
}
