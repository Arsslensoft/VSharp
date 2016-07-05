namespace VSC.AST
{
    public class FloatLiteral : FloatConstant, ILiteralConstant
    {
        public FloatLiteral( float f, Location loc)
            : base( f, loc)
        {
        }

        public override bool IsLiteral
        {
            get { return true; }
        }


        public char[] ParsedValue { get; set; }

    }
}