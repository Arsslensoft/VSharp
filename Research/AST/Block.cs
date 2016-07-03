using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Context;

namespace VSC.AST
{
    public class Block : Statement
    {
        [Flags]
        public enum Flags
        {
            Unchecked = 1,
            ReachableEnd = 8,
            Unsafe = 16,
            HasCapturedVariable = 64,
            HasCapturedThis = 1 << 7,
            IsExpressionTree = 1 << 8,
            CompilerGenerated = 1 << 9,
            HasAsyncModifier = 1 << 10,
            Resolved = 1 << 11,
            YieldBlock = 1 << 12,
            AwaitBlock = 1 << 13,
            FinallyBlock = 1 << 14,
            CatchBlock = 1 << 15,
            Iterator = 1 << 20,
            NoFlowAnalysis = 1 << 21,
            InitializationEmitted = 1 << 22
        }

        public Block Parent;
        public Location StartLocation;
        public Location EndLocation;

        public ExplicitBlock Explicit;
        public ParametersBlock ParametersBlock;

        protected Flags flags;
        Block original;
        //
        // The statements in this block
        //
        protected List<Statement> statements;
        public Block(Block parent, Location start, Location end)
            : this(parent, 0, start, end)
        {
        }

        public Block(Block parent, Flags flags, Location start, Location end)
        {
            if (parent != null)
            {
                // the appropriate constructors will fixup these fields
                ParametersBlock = parent.ParametersBlock;
                Explicit = parent.Explicit;
            }

            this.Parent = parent;
            this.flags = flags;
            this.StartLocation = start;
            this.EndLocation = end;
            this.loc = start;
            statements = new List<Statement>(4);

            this.original = this;
        }

        public void SetEndLocation(Location loc)
        {
            EndLocation = loc;
        }
        #region Properties

        public Block Original
        {
            get
            {
                return original;
            }
            protected set
            {
                original = value;
            }
        }

        public bool IsCompilerGenerated
        {
            get { return (flags & Flags.CompilerGenerated) != 0; }
            set { flags = value ? flags | Flags.CompilerGenerated : flags & ~Flags.CompilerGenerated; }
        }


        public bool IsCatchBlock
        {
            get
            {
                return (flags & Flags.CatchBlock) != 0;
            }
        }

        public bool IsFinallyBlock
        {
            get
            {
                return (flags & Flags.FinallyBlock) != 0;
            }
        }

        public bool Unchecked
        {
            get { return (flags & Flags.Unchecked) != 0; }
            set { flags = value ? flags | Flags.Unchecked : flags & ~Flags.Unchecked; }
        }

        public bool Unsafe
        {
            get { return (flags & Flags.Unsafe) != 0; }
            set { flags |= Flags.Unsafe; }
        }

        public List<Statement> Statements
        {
            get { return statements; }
        }

        #endregion
        public void AddStatement(Statement s)
        {
            statements.Add(s);
        }
        public void AddLocalName(LocalVariable lv)
        {

        }
    }
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
    }
    //
	// ParametersBlock was introduced to support anonymous methods
	// and lambda expressions
	// 
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
    }
}
