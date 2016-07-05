using System;

namespace VSC.AST
{
    public class ParametersBlock : ExplicitBlock
    {
        protected ParametersCompiled parameters;
        protected ToplevelBlock top_block;
        public ParametersBlock(Block parent, ParametersCompiled parameters, Location start, Flags flags = 0)
            : base(parent, 0, start, start)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            this.parameters = parameters;
            ParametersBlock = this;
            this.flags |= flags | (parent.ParametersBlock.flags & (Flags.YieldBlock | Flags.AwaitBlock));
            this.top_block = parent.ParametersBlock.top_block;
        }

        protected ParametersBlock(ParametersCompiled parameters, Location start)
            : base(null, 0, start, start)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            this.parameters = parameters;
            ParametersBlock = this;
        }

        //
        // It's supposed to be used by method body implementation of anonymous methods
        //
        protected ParametersBlock(ParametersBlock source, ParametersCompiled parameters)
            : base(null, 0, source.StartLocation, source.EndLocation)
        {
            this.parameters = parameters;
            this.statements = source.statements;
            this.reachable = source.reachable;
            this.flags = source.flags & Flags.ReachableEnd;
            ParametersBlock = this;
            //
            // Overwrite original for comparison purposes when linking cross references
            // between anonymous methods
            //
            Original = source.Original;
        }
        public ToplevelBlock TopBlock
        {
            get
            {
                return top_block;
            }
            set
            {
                top_block = value;
            }
        }

    }
}