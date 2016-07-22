namespace VSC.AST
{
    public class ExplicitBlock : Block
    {
      //  protected AnonymousMethodStorey am_storey;
        public ExplicitBlock(Block parent, Location start, Location end)
            : this(parent, (Flags)0, start, end)
        {
        }

        public ExplicitBlock(Block parent, Flags flags, Location start, Location end)
            : base(parent, flags, start, end)
        {
            this.Explicit = this;
        }

        #region Properties
/*
        public AnonymousMethodStorey AnonymousMethodStorey
        {
            get
            {
                return am_storey;
            }
        }*/

        public bool HasAwait
        {
            get
            {
                return (flags & Flags.AwaitBlock) != 0;
            }
        }

        public bool HasCapturedThis
        {
            set
            {
                flags = value ? flags | Flags.HasCapturedThis : flags & ~Flags.HasCapturedThis;
            }
            get
            {
                return (flags & Flags.HasCapturedThis) != 0;
            }
        }

        //
        // Used to indicate that the block has reference to parent
        // block and cannot be made static when defining anonymous method
        //
        public bool HasCapturedVariable
        {
            set
            {
                flags = value ? flags | Flags.HasCapturedVariable : flags & ~Flags.HasCapturedVariable;
            }
            get
            {
                return (flags & Flags.HasCapturedVariable) != 0;
            }
        }

        public bool HasReachableClosingBrace
        {
            get
            {
                return (flags & Flags.ReachableEnd) != 0;
            }
            set
            {
                flags = value ? flags | Flags.ReachableEnd : flags & ~Flags.ReachableEnd;
            }
        }

        public bool HasYield
        {
            get
            {
                return (flags & Flags.YieldBlock) != 0;
            }
        }

        #endregion


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