using VSC.TypeSystem;

namespace VSC.AST
{
    public abstract class IntegralConstant : Constant
    {
        protected IntegralConstant(ITypeReference type, Location loc)
            : base(loc)
        {
            this.type = type;
       
        }

        public override string GetValueAsLiteral()
        {
            return GetValue().ToString();
        }

        public abstract Constant Increment();
    }
}