using System.Collections.Generic;

namespace VSC.AST
{
    public class NewAnonymousTypeExpression : NewExpression
    {
        static readonly AnonymousTypeParameter[] EmptyParameters = new AnonymousTypeParameter[0];

        List<AnonymousTypeParameter> parameters;
        readonly TypeContainer parent;

        public NewAnonymousTypeExpression(List<AnonymousTypeParameter> parameters, TypeContainer parent, Location loc)
            : base (null, null, loc)
        {
            this.parameters = parameters;
            this.parent = parent;
        }

        public List<AnonymousTypeParameter> Parameters {
            get {
                return this.parameters;
            }
        }
    }
}