namespace VSC.AST
{
    public class IntLiteral : IntConstant, ILiteralConstant
    {
        public IntLiteral(int l, Location loc)
            : base(l, loc)
        {
        }
        public override bool IsLiteral
        {
            get { return true; }
        }

        public char[] ParsedValue { get; set; }

    }
}