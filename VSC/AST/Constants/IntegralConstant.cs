using VSC.TypeSystem;

namespace VSC.AST
{
    public abstract class IntegralConstant : Constant
    {
        protected IntegralConstant(ITypeReference UnresolvedTypeReference, Location loc)
            : base(loc)
        {
            this.type = UnresolvedTypeReference;
       
        }

        public override string GetValueAsLiteral()
        {
            return GetValue().ToString();
        }

        public abstract Constant Increment();
    }
}