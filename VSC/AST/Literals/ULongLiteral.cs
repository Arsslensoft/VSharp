namespace VSC.AST
{
    public class ULongLiteral : ULongConstant, ILiteralConstant
    {
        public ULongLiteral(ulong l, Location loc)
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