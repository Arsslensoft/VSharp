using System;
using System.Collections.Generic;
using VSC.Context;

namespace VSC.AST
{
    public class ToplevelBlock : ParametersBlock
    {
        LocalVariable this_variable;
        CompilerContext compiler;
        Dictionary<string, object> names;

        List<ExplicitBlock> this_references;

        public ToplevelBlock(CompilerContext ctx, Location loc)
            : this(ctx, ParametersCompiled.EmptyReadOnlyParameters, loc)
        {
        }

        public ToplevelBlock(CompilerContext ctx, ParametersCompiled parameters, Location start, Flags flags = 0)
            : base(parameters, start)
        {
            this.compiler = ctx;
            this.flags = flags;
            top_block = this;

       
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
        //
        // Used by anonymous blocks to track references of `this' variable
        //
        public List<ExplicitBlock> ThisReferencesFromChildrenBlock
        {
            get
            {
                return this_references;
            }
        }
        public void AddThisReferenceFromChildrenBlock(ExplicitBlock block)
        {
            if (this_references == null)
                this_references = new List<ExplicitBlock>();

            if (!this_references.Contains(block))
                this_references.Add(block);
        }
        public void RemoveThisReferenceFromChildrenBlock(ExplicitBlock block)
        {
            this_references.Remove(block);
        }
     
        //
        // Returns the "this" instance variable of this block.
        // See AddThisVariable() for more information.
        //
        public LocalVariable ThisVariable
        {
            get
            {
                return this_variable;
            }
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