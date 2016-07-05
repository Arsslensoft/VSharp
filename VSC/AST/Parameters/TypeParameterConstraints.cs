using System.Collections.Generic;

namespace VSC.AST
{
    public class TypeParameterConstraints
    {
        readonly SimpleMemberName tparam;
        readonly List<FullNamedExpression> constraints;
        readonly Location loc;
        bool resolved;
        bool resolving;

        public TypeParameterConstraints(SimpleMemberName tparam, List<FullNamedExpression> constraints, Location loc)
        {
            this.tparam = tparam;
            this.constraints = constraints;
            this.loc = loc;
        }
        #region Properties

        public List<FullNamedExpression> TypeExpressions
        {
            get
            {
                return constraints;
            }
        }

        public Location Location
        {
            get
            {
                return loc;
            }
        }

        public SimpleMemberName TypeParameter
        {
            get
            {
                return tparam;
            }
        }

        #endregion
    }
}