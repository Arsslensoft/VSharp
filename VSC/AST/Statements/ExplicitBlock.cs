namespace VSC.AST
{
    public class ExplicitBlock : Block
    {

        public ExplicitBlock(Block parent, Location start, Location end)
            : this(parent, (Flags)0, start, end)
        {
        }

        public ExplicitBlock(Block parent, Flags flags, Location start, Location end)
            : base(parent, flags, start, end)
        {
            this.Explicit = this;
        }


        public void RegisterIteratorYield()
        {
            ParametersBlock.TopBlock.IsIterator = true;

            var block = this;
            while ((block.flags & Flags.YieldBlock) == 0)
            {
                block.flags |= Flags.YieldBlock;

                if (block.Parent == null)
                    return;

                block = block.Parent.Explicit;
            }
        }
    }
}