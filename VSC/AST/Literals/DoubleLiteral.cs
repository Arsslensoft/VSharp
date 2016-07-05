namespace VSC.AST
{
    public class DoubleLiteral : DoubleConstant, ILiteralConstant
    {
        public DoubleLiteral(double d, Location loc)
            : base(d, loc)
        {
        }

    
        public override bool IsLiteral
        {
            get { return true; }
        }

        public char[] ParsedValue { get; set; }

    }
}