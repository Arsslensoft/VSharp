namespace VSC.AST
{
    public class CharLiteral : CharConstant, ILiteralConstant
    {
        public CharLiteral( char c, Location loc)
            : base(c, loc)
        {
        }

        public override bool IsLiteral
        {
            get { return true; }
        }

        public char[] ParsedValue { get; set; }

    }
}