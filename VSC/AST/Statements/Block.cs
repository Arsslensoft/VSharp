using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void AddLabel(LabeledStatement lb)
        {

        }
    }

    //
	// ParametersBlock was introduced to support anonymous methods
	// and lambda expressions
	// 
}
