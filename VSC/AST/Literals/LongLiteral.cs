namespace VSC.AST
{
    public class LongLiteral : LongConstant, ILiteralConstant
    {
        public LongLiteral( long l, Location loc)
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