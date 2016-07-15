using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   Implements the typeof operator
    /// </summary>
    public class TypeOfExpression : Expression
    {
        FullNamedExpression QueriedType;

        public TypeOfExpression(FullNamedExpression queried_type, Location l)
        {
            QueriedType = queried_type;
            loc = l;
        }


        #region Properties

        public FullNamedExpression TypeExpression
        {
            get
            {
                return QueriedType;
            }
        }

        #endregion

        public override IConstantValue BuilConstantValue(bool isAttributeConstant)
        {
            if (isAttributeConstant)
            {
                return new TypeOfConstantExpression(QueriedType as ITypeReference);
            }
            else
            {
                return null;
            }
        }
    }
}