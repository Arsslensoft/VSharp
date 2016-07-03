namespace VSC.AST {

//
	// The information about a user-perceived local variable
	//
	public sealed class LocalVariable : INamedBlockVariable, ILocalVariable
	{
		/*[Flags]
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
			SymbolFileHidden = 1 << 9,

			ReadonlyMask = ForeachVariable | FixedVariable | UsingVariable
		}

		TypeSpec type;
		readonly string name;
		readonly Location loc;
		readonly Block block;
		Flags flags;
		Constant const_value;

		public VariableInfo VariableInfo;
		HoistedVariable hoisted_variant;

		internal LocalBuilder builder;

		public LocalVariable (Block block, string name, Location loc)
		{
			this.block = block;
			this.name = name;
			this.loc = loc;
		}

		public LocalVariable (Block block, string name, Flags flags, Location loc)
			: this (block, name, loc)
		{
			this.flags = flags;
		}

		//
		// Used by variable declarators
		//
		public LocalVariable (LocalVariable li, string name, Location loc)
			: this (li.block, name, li.flags, loc)
		{
		}

		#region Properties

		public bool AddressTaken {
			get {
				return (flags & Flags.AddressTaken) != 0;
			}
		}

		public Block Block {
			get {
				return block;
			}
		}

		public Constant ConstantValue {
			get {
				return const_value;
			}
			set {
				const_value = value;
			}
		}

		//
		// Hoisted local variable variant
		//
		public HoistedVariable HoistedVariant {
			get {
				return hoisted_variant;
			}
			set {
				hoisted_variant = value;
			}
		}

		public bool IsDeclared {
			get {
				return type != null;
			}
		}

		public bool IsCompilerGenerated {
			get {
				return (flags & Flags.CompilerGenerated) != 0;
			}
		}

		public bool IsConstant {
			get {
				return (flags & Flags.Constant) != 0;
			}
		}

		public bool IsLocked {
			get {
				return (flags & Flags.IsLocked) != 0;
			}
			set {
				flags = value ? flags | Flags.IsLocked : flags & ~Flags.IsLocked;
			}
		}

		public bool IsThis {
			get {
				return (flags & Flags.IsThis) != 0;
			}
		}

		public bool IsFixed {
			get {
				return (flags & Flags.FixedVariable) != 0;
			}
			set {
				flags = value ? flags | Flags.FixedVariable : flags & ~Flags.FixedVariable;
			}
		}

		bool INamedBlockVariable.IsParameter {
			get {
				return false;
			}
		}

		public bool IsReadonly {
			get {
				return (flags & Flags.ReadonlyMask) != 0;
			}
		}

		public Location Location {
			get {
				return loc;
			}
		}

		public string Name {
			get {
				return name;
			}
		}

		public TypeSpec Type {
		    get {
				return type;
			}
		    set {
				type = value;
			}
		}

		#endregion

		public void CreateBuilder (EmitContext ec)
		{
			if ((flags & Flags.Used) == 0) {
				if (VariableInfo == null) {
					// Missing flow analysis or wrong variable flags
					throw new InternalErrorException ("VariableInfo is null and the variable `{0}' is not used", name);
				}

				if (VariableInfo.IsEverAssigned)
					ec.Report.Warning (219, 3, Location, "The variable `{0}' is assigned but its value is never used", Name);
				else
					ec.Report.Warning (168, 3, Location, "The variable `{0}' is declared but never used", Name);
			}

			if (HoistedVariant != null)
				return;

			if (builder != null) {
				if ((flags & Flags.CompilerGenerated) != 0)
					return;

				// To avoid Used warning duplicates
				throw new InternalErrorException ("Already created variable `{0}'", name);
			}

			//
			// All fixed variabled are pinned, a slot has to be alocated
			//
			builder = ec.DeclareLocal (Type, IsFixed);
			if ((flags & Flags.SymbolFileHidden) == 0)
				ec.DefineLocalVariable (name, builder);
		}

		public static LocalVariable CreateCompilerGenerated (TypeSpec type, Block block, Location loc, bool writeToSymbolFile = false)
		{
			LocalVariable li = new LocalVariable (block, GetCompilerGeneratedName (block), Flags.CompilerGenerated | Flags.Used, loc);
			if (!writeToSymbolFile)
				li.flags |= Flags.SymbolFileHidden;
			
			li.Type = type;
			return li;
		}

		public Expression CreateReferenceExpression (ResolveContext rc, Location loc)
		{
			if (IsConstant && const_value != null)
				return Constant.CreateConstantFromValue (Type, const_value.GetValue (), loc);

			return new LocalVariableReference (this, loc);
		}

		public void Emit (EmitContext ec)
		{
			// TODO: Need something better for temporary variables
			if ((flags & Flags.CompilerGenerated) != 0)
				CreateBuilder (ec);

			ec.Emit (OpCodes.Ldloc, builder);
		}

		public void EmitAssign (EmitContext ec)
		{
			// TODO: Need something better for temporary variables
			if ((flags & Flags.CompilerGenerated) != 0)
				CreateBuilder (ec);

			ec.Emit (OpCodes.Stloc, builder);
		}

		public void EmitAddressOf (EmitContext ec)
		{
			// TODO: Need something better for temporary variables
			if ((flags & Flags.CompilerGenerated) != 0)
				CreateBuilder (ec);

			ec.Emit (OpCodes.Ldloca, builder);
		}

		public static string GetCompilerGeneratedName (Block block)
		{
			// HACK: Debugger depends on the name semantics
			return "$locvar" + block.ParametersBlock.TemporaryLocalsCount++.ToString ("X");
		}

		public string GetReadOnlyContext ()
		{
			switch (flags & Flags.ReadonlyMask) {
			case Flags.FixedVariable:
				return "fixed variable";
			case Flags.ForeachVariable:
				return "foreach iteration variable";
			case Flags.UsingVariable:
				return "using variable";
			}

			throw new InternalErrorException ("Variable is not readonly");
		}

		public bool IsThisAssigned (FlowAnalysisContext fc, Block block)
		{
			if (VariableInfo == null)
				throw new Exception ();

			if (IsAssigned (fc))
				return true;

			return VariableInfo.IsFullyInitialized (fc, block.StartLocation);
		}

		public bool IsAssigned (FlowAnalysisContext fc)
		{
			return fc.IsDefinitelyAssigned (VariableInfo);
		}

		public void PrepareAssignmentAnalysis (BlockContext bc)
		{
			//
			// No need to run assignment analysis for these guys
			//
			if ((flags & (Flags.Constant | Flags.ReadonlyMask | Flags.CompilerGenerated)) != 0)
				return;

			VariableInfo = VariableInfo.Create (bc, this);
		}

		//
		// Mark the variables as referenced in the user code
		//
		public void SetIsUsed ()
		{
			flags |= Flags.Used;
		}

		public void SetHasAddressTaken ()
		{
			flags |= (Flags.AddressTaken | Flags.Used);
		}

		public override string ToString ()
		{
			return string.Format ("LocalInfo ({0},{1},{2},{3})", name, type, VariableInfo, Location);
		}*/
	}

	
}