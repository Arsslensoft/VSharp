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

    }
}