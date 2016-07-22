using VSC.TypeSystem;

namespace VSC.AST
{
    public class Argument 
    {

        public IAstNode ParentNode { get; set; }
    
        public enum AType : byte
        {
            None = 0,
            Ref = 1,			// ref modifier used
            Out = 2,			// out modifier used
            Default = 3,		// argument created from default parameter value
            DynamicTypeName = 4,	// System.Type argument for dynamic binding
            ExtensionType = 5,	// Instance expression inserted as the first argument

            // Conditional instance expression inserted as the first argument
            ExtensionTypeConditionalAccess = 5 | ConditionalAccessFlag,

            ConditionalAccessFlag = 1 << 7
        }

        public readonly AType ArgType;
        public Expression Expr;
        public readonly Location loc;
        public Argument(Expression expr, AType type, Location l)
        {
            this.Expr = expr;
            this.ArgType = type;
          
            this.loc = l;
        }

        public Argument(Expression expr, Location l)
        {
            this.Expr = expr; this.loc = l;
        }

        public static explicit operator Expression(Argument a)
        {
            return a.Expr;
        }
        public void Resolve(VSC.TypeSystem.Resolver.ResolveContext rc)
        {
            // Verify that the argument is readable
            if (ArgType != AType.Out)
                Expr = Expr.DoResolve(rc);

            // Verify that the argument is writeable
            if (Expr != null && IsByRef)
                Expr = Expr.DoResolveLeftValue(rc, EmptyExpression.OutAccess);

            if (Expr == null)
                Expr = Expression.ErrorResult;


        }
        #region Properties

        public bool IsByRef
        {
            get { return ArgType == AType.Ref || ArgType == AType.Out; }
        }

        public bool IsDefaultArgument
        {
            get { return ArgType == AType.Default; }
        }

        public bool IsExtensionType
        {
            get
            {
                return (ArgType & AType.ExtensionType) == AType.ExtensionType;
            }
        }

        public ParameterModifier Modifier
        {
            get
            {
                switch (ArgType)
                {
                    case AType.Out:
                        return ParameterModifier.Out;

                    case AType.Ref:
                        return ParameterModifier.Ref;

                    default:
                        return ParameterModifier.None;
                }
            }
        }


        #endregion
    }
}