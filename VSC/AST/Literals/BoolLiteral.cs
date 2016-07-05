namespace VSC.AST
{
    public class BoolLiteral : BoolConstant, ILiteralConstant
    {
        public BoolLiteral(bool val, Location loc)
            : base(val, loc)
        {
        }

        public override bool IsLiteral
        {
            get { return true; }
        }

        public char[] ParsedValue { get; set; }

    }
}