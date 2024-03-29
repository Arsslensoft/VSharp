using System.Diagnostics;
using VSC.TypeSystem;

namespace VSC.AST {

    //
    // The information about a user-perceived local variable
    //
    public sealed class LocalVariable : INamedBlockVariable, IVariable
    {
        readonly DomRegion region;
     internal   IType type;
        readonly string name;
        [System.Flags]
        public enum Flags
        {
            Used = 1,
            IsThis = 1 << 1,
            AddressTaken = 1 << 2,
            CompilerGenerated = 1 << 3,
            Constant = 1 << 4,
            ForeachVariable = 1 << 5,
            FixedVariable = 1 << 6,
            UsingVariable = 1 << 7,
            IsLocked = 1 << 8,

            ReadonlyMask = ForeachVariable | FixedVariable | UsingVariable
        }

        readonly Location loc;
        readonly Block block;
        Flags flags;
        Constant const_value;

        public VariableInfo VariableInfo;
       // HoistedVariable hoisted_variant;


        public LocalVariable( IType type, string name)
			{
				Debug.Assert(type != null);
				Debug.Assert(name != null);
				this.type = type;
				this.name = name;
			}
			
			public SymbolKind SymbolKind {
				get { return SymbolKind.Variable; }
			}
			
			public string Name {
				get { return name; }
			}
			
			public DomRegion Region {
				get { return region; }
			}
			
			public IType Type {
				get { return type; }
               
			}
			
			public bool IsConst {
				get { return false; }
			}
			
			public object ConstantValue {
				get { return null; }
			}
			
			public override string ToString()
			{
				return type.ToString() + " " + name + ";";
			}

			public ISymbolReference ToReference()
			{
                return new VSC.TypeSystem.Implementation.VariableReference(type.ToTypeReference(), name, region, IsConst, ConstantValue);
			}
        public LocalVariable(Block block, string name, Location loc)
        {
            this.block = block;
            this.name = name;
            this.loc = loc;
        }

        public LocalVariable(Block block, string name, Flags flags, Location loc)
            : this(block, name, loc)
        {
            this.flags = flags;
        }

        //
        // Used by variable declarators
        //
        public LocalVariable(LocalVariable li, string name, Location loc)
            : this(li.block, name, li.flags, loc)
        {
        }

        #region Properties

        public bool AddressTaken
        {
            get
            {
                return (flags & Flags.AddressTaken) != 0;
            }
        }

        public Block Block
        {
            get
            {
                return block;
            }
        }

   

      /*  //
        // Hoisted local variable variant
        //
        public HoistedVariable HoistedVariant
        {
            get
            {
                return hoisted_variant;
            }
            set
            {
                hoisted_variant = value;
            }
        }*/

        public bool IsDeclared
        {
            get
            {
                return type != null;
            }
        }

        public bool IsCompilerGenerated
        {
            get
            {
                return (flags & Flags.CompilerGenerated) != 0;
            }
        }

        public bool IsConstant
        {
            get
            {
                return (flags & Flags.Constant) != 0;
            }
        }

        public bool IsLocked
        {
            get
            {
                return (flags & Flags.IsLocked) != 0;
            }
            set
            {
                flags = value ? flags | Flags.IsLocked : flags & ~Flags.IsLocked;
            }
        }

        public bool IsThis
        {
            get
            {
                return (flags & Flags.IsThis) != 0;
            }
        }

        public bool IsFixed
        {
            get
            {
                return (flags & Flags.FixedVariable) != 0;
            }
        }

        bool INamedBlockVariable.IsParameter
        {
            get
            {
                return false;
            }
        }

        public bool IsReadonly
        {
            get
            {
                return (flags & Flags.ReadonlyMask) != 0;
            }
        }

        public Location Location
        {
            get
            {
                return loc;
            }
        }

     

        #endregion

        public static LocalVariable CreateCompilerGenerated(IType type, Block block, Location loc)
        {
            LocalVariable li = new LocalVariable(block, GetCompilerGeneratedName(block), Flags.CompilerGenerated | Flags.Used, loc);
            li.type = type;
            return li;
        }
        public static string GetCompilerGeneratedName(Block block)
        {
            // HACK: Debugger depends on the name semantics
            return "$locvar" + block.ParametersBlock.TemporaryLocalsCount++.ToString("X");
        }

        public string GetReadOnlyContext()
        {
            switch (flags & Flags.ReadonlyMask)
            {
                case Flags.ForeachVariable:
                    return "foreach iteration variable";
                case Flags.UsingVariable:
                    return "using variable";
            }

            throw new InternalErrorException("Variable is not readonly");
        }

  
    }

	
}