using System.Collections.Generic;

namespace VSC.TypeSystem.Resolver
{
    /// <summary>
    /// A method list that belongs to a declaring type.
    /// </summary>
    public class MethodListWithDeclaringType : List<IParameterizedMember>
    {
        readonly IType declaringType;

        /// <summary>
        /// The declaring type.
        /// </summary>
        /// <remarks>
        /// LogicalNot all methods in this list necessarily have this as their declaring type.
        /// For example, this program:
        /// <code>
        ///  class Base {
        ///    public virtual void M() {}
        ///  }
        ///  class Derived : Base {
        ///    public override void M() {}
        ///    public void M(int i) {}
        ///  }
        /// </code>
        /// results in two lists:
        ///  <c>new MethodListWithDeclaringType(Base) { Derived.M() }</c>,
        ///  <c>new MethodListWithDeclaringType(Derived) { Derived.M(int) }</c>
        /// </remarks>
        public IType DeclaringType
        {
            get { return declaringType; }
        }

        public MethodListWithDeclaringType(IType declaringType)
        {
            this.declaringType = declaringType;
        }

        public MethodListWithDeclaringType(IType declaringType, IEnumerable<IParameterizedMember> methods)
            : base(methods)
        {
            this.declaringType = declaringType;
        }
    }
}