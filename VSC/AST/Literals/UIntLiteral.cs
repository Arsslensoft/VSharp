namespace VSC.AST
{
    public class UIntLiteral : UIntConstant, ILiteralConstant
    {
        public UIntLiteral(uint l, Location loc)
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