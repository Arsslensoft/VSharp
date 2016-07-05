using System;
using System.Collections.Generic;
using System.Text;
using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
    public abstract class SpecializedParameterizedMember : SpecializedMember, IParameterizedMember
    {
        IList<IParameter> parameters;
		
        protected SpecializedParameterizedMember(IParameterizedMember memberDefinition)
            : base(memberDefinition)
        {
        }
		
        public IList<IParameter> Parameters {
            get {
                var result = LazyInit.VolatileRead(ref this.parameters);
                if (result != null)
                    return result;
                else
                    return LazyInit.GetOrSet(ref this.parameters, CreateParameters(this.Substitution));
            }
            protected set {
                // This setter is used for LiftedUserDefinedOperator, a special case of specialized member
                // (not a normal type parameter substitution).
				
                // As this setter is used only during construction before the member is published
                // to other threads, we don't need a volatile write.
                this.parameters = value;
            }
        }
		
        protected IList<IParameter> CreateParameters(TypeVisitor substitution)
        {
            var paramDefs = ((IParameterizedMember)this.baseMember).Parameters;
            if (paramDefs.Count == 0) {
                return EmptyList<IParameter>.Instance;
            } else {
                var parameters = new IParameter[paramDefs.Count];
                for (int i = 0; i < parameters.Length; i++) {
                    var p = paramDefs[i];
                    IType newType = p.Type.AcceptVisitor(substitution);
                    parameters[i] = new ParameterSpec(
                        newType, p.Name, this,
                        p.Region, p.Attributes, p.IsRef, p.IsOut,
                        p.IsParams, p.IsOptional, p.ConstantValue
                        );
                }
                return Array.AsReadOnly(parameters);
            }
        }
		
        public override string ToString()
        {
            StringBuilder b = new StringBuilder("[");
            b.Append(GetType().Name);
            b.Append(' ');
            b.Append(this.DeclaringType.ReflectionName);
            b.Append('.');
            b.Append(this.Name);
            b.Append('(');
            for (int i = 0; i < this.Parameters.Count; i++) {
                if (i > 0) b.Append(", ");
                b.Append(this.Parameters[i].ToString());
            }
            b.Append("):");
            b.Append(this.ReturnType.ReflectionName);
            b.Append(']');
            return b.ToString();
        }
    }
}