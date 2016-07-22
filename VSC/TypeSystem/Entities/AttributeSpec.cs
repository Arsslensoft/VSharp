using System;
using System.Collections.Generic;
using System.Linq;
using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// IAttribute implementation for already-resolved attributes.
	/// </summary>
	public class AttributeSpec : IAttribute
	{
		readonly IType attributeType;
        readonly IList<AST.Expression> positionalArguments;
        readonly IList<KeyValuePair<IMember, AST.Expression>> namedArguments;
		readonly DomRegion region;
		volatile IMethod constructor;

        public AttributeSpec(IType attributeType, IList<AST.Expression> positionalArguments = null,
                                IList<KeyValuePair<IMember, AST.Expression>> namedArguments = null,
		                        DomRegion region = default(DomRegion))
		{
			if (attributeType == null)
				throw new ArgumentNullException("attributeType");
			this.attributeType = attributeType;
            this.positionalArguments = positionalArguments ?? EmptyList<AST.Expression>.Instance;
            this.namedArguments = namedArguments ?? EmptyList<KeyValuePair<IMember, AST.Expression>>.Instance;
			this.region = region;
		}

        public AttributeSpec(IMethod constructor, IList<AST.Expression> positionalArguments = null,
                                IList<KeyValuePair<IMember, AST.Expression>> namedArguments = null,
		                        DomRegion region = default(DomRegion))
		{
			if (constructor == null)
				throw new ArgumentNullException("constructor");
			this.constructor = constructor;
			this.attributeType = constructor.DeclaringType ?? SpecialTypeSpec.UnknownType;
            this.positionalArguments = positionalArguments ?? EmptyList<AST.Expression>.Instance;
            this.namedArguments = namedArguments ?? EmptyList<KeyValuePair<IMember, AST.Expression>>.Instance;
			this.region = region;
			if (this.positionalArguments.Count != constructor.Parameters.Count) {
				throw new ArgumentException("Positional argument count must match the constructor's parameter count");
			}
		}
		
		public IType AttributeType {
			get { return attributeType; }
		}
		
		public DomRegion Region {
			get { return region; }
		}
		
		public IMethod Constructor {
			get {
				IMethod ctor = this.constructor;
				if (ctor == null) {
					foreach (IMethod candidate in this.AttributeType.GetConstructors(m => m.Parameters.Count == positionalArguments.Count)) {
						if (candidate.Parameters.Select(p => p.Type).SequenceEqual(this.PositionalArguments.Select(a => a.Type))) {
							ctor = candidate;
							break;
						}
					}
					this.constructor = ctor;
				}
				return ctor;
			}
		}

        public IList<AST.Expression> PositionalArguments
        {
			get { return positionalArguments; }
		}

        public IList<KeyValuePair<IMember, AST.Expression>> NamedArguments
        {
			get { return namedArguments; }
		}
	}
}
