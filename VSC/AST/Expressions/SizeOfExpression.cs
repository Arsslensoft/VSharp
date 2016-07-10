using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   Implements the sizeof expression
    /// </summary>
    public class SizeOfExpression : Expression
    {
        readonly Expression texpr;


        public SizeOfExpression(Expression queried_type, Location l)
        {
            this.texpr = queried_type;
            loc = l;
        }
        public Expression TypeExpression
        {
            get
            {
                return texpr;
            }
        }

        public override IConstantValue BuilConstantValue(ResolveContext rc, bool isAttributeConstant)
        {
            return new SizeOfConstantValue(TypeExpression as ITypeReference);
        }
    }
}