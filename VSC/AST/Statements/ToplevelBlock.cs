using System;
using VSC.Context;

namespace VSC.AST
{
    public class ToplevelBlock : ParametersBlock
    {
        CompilerContext compiler;
        public ToplevelBlock(CompilerContext ctx, Location loc)
            : this(ctx, ParametersCompiled.EmptyReadOnlyParameters, loc)
        {
        }

        public ToplevelBlock(CompilerContext ctx, ParametersCompiled parameters, Location start, Flags flags = 0)
            : base(parameters, start)
        {
            this.compiler = ctx;
            this.flags = flags;
        }

        //
        // Recreates a top level block from parameters block. Used for
        // compiler generated methods where the original block comes from
        // explicit child block. This works for already resolved blocks
        // only to ensure we resolve them in the correct flow order
        //
        public ToplevelBlock(ParametersBlock source, ParametersCompiled parameters)
            : base(source, parameters)
        {
            this.compiler = source.TopBlock.compiler;
            top_block = this;
        }

        public void AddLocalName(string name, INamedBlockVariable li, bool ignoreChildrenBlocks)
        {
            throw new NotSupportedException();
        }

        public bool IsIterator
        {
            get
            {
                return (flags & Flags.Iterator) != 0;
            }
            set
            {
                flags = value ? flags | Flags.Iterator : flags & ~Flags.Iterator;
            }
        }

    }
}