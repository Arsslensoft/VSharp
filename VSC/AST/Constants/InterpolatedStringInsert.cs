namespace VSC.AST
{
    public class InterpolatedStringInsert : CompositeExpression
    {
        public InterpolatedStringInsert(Expression expr)
            : base(expr)
        {
        }

        public Expression Alignment { get; set; }
        public string Format { get; set; }
    }
}