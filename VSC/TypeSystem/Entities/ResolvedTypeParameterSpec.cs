
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
	public class ResolvedTypeParameterSpec : TypeParameterSpec
	{
		readonly bool hasValueTypeConstraint;
		readonly bool hasReferenceTypeConstraint;
		readonly IList<IType> constraints;
		
		public ResolvedTypeParameterSpec(
			IEntity owner,
			int index, string name = null,
			DomRegion region = default(DomRegion),
			bool hasValueTypeConstraint = false, bool hasReferenceTypeConstraint = false, 
			IList<IType> constraints = null)
			: base(owner, index, name, region)
		{
			this.hasValueTypeConstraint = hasValueTypeConstraint;
			this.hasReferenceTypeConstraint = hasReferenceTypeConstraint;
			this.constraints = constraints ?? EmptyList<IType>.Instance;
		}
		
		public ResolvedTypeParameterSpec(
			ICompilation compilation, SymbolKind ownerType,
			int index, string name = null,
			DomRegion region = default(DomRegion),
			bool hasValueTypeConstraint = false, bool hasReferenceTypeConstraint = false,
			IList<IType> constraints = null)
			: base(compilation, ownerType, index, name,  region)
		{
			this.hasValueTypeConstraint = hasValueTypeConstraint;
			this.hasReferenceTypeConstraint = hasReferenceTypeConstraint;
		
			this.constraints = constraints ?? EmptyList<IType>.Instance;
		}
		
		public override bool HasValueTypeConstraint {
			get { return hasValueTypeConstraint; }
		}
		
		public override bool HasReferenceTypeConstraint {
			get { return hasReferenceTypeConstraint; }
		}
		
	
		
		public override IEnumerable<IType> DirectBaseTypes {
			get {
				bool hasNonInterfaceConstraint = false;
				foreach (IType c in constraints) {
					yield return c;
					if (c.Kind != TypeKind.Interface)
						hasNonInterfaceConstraint = true;
				}
				// Do not add the 'System.Object' constraint if there is another constraint with a base class.
				if (this.HasValueTypeConstraint || !hasNonInterfaceConstraint) {
					yield return this.Compilation.FindType(this.HasValueTypeConstraint ? KnownTypeCode.ValueType : KnownTypeCode.Object);
				}
			}
		}
	}
	
	
}
