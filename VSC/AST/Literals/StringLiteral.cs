namespace VSC.AST
{
    public class StringLiteral : StringConstant, ILiteralConstant
    {
        public StringLiteral(string s, Location loc)
            : base(s, loc)
        {
        }

        public override bool IsLiteral
        {
            get { return true; }
        }

        public char[] ParsedValue { get; set; }

    }
}