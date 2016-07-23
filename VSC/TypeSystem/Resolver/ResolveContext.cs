using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using VSC.AST;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem.Implementation;
using Expression = VSC.AST.Expression;

namespace VSC.TypeSystem.Resolver
{
    //TODO:Support custom operators

    /// <summary>
	/// Contains the main resolver logic.
	/// </summary>
	/// <remarks>
	/// This class is thread-safe.
	/// </remarks>
	public partial class ResolveContext : ICodeContext
	{
  
        [Flags]
        public enum Options
        {
            /// <summary>
            ///   This flag tracks the `checked' state of the compilation,
            ///   it controls whether we should generate code that does overflow
            ///   checking, or if we generate code that ignores overflows.
            ///
            ///   The default setting comes from the command line option to generate
            ///   checked or unchecked code plus any source code changes using the
            ///   checked/unchecked statements or expressions.   Contrast this with
            ///   the ConstantCheckState flag.
            /// </summary>
            CheckedScope = 1 << 0,

            /// <summary>
            ///   The constant check state is always set to `true' and cant be changed
            ///   from the command line.  The source code can change this setting with
            ///   the `checked' and `unchecked' statements and expressions. 
            /// </summary>
            ConstantCheckState = 1 << 1,

            AllCheckStateFlags = CheckedScope | ConstantCheckState,

            //
            // unsafe { ... } scope
            //
            UnsafeScope = 1 << 2,
            CatchScope = 1 << 3,
            FinallyScope = 1 << 4,
            FieldInitializerScope = 1 << 5,
            CompoundAssignmentScope = 1 << 6,
            FixedInitializerScope = 1 << 7,
            BaseInitializer = 1 << 8,

            //
            // Inside an enum definition, we do not resolve enumeration values
            // to their enumerations, but rather to the underlying type/value
            // This is so EnumVal + EnumValB can be evaluated.
            //
            // There is no "E operator + (E x, E y)", so during an enum evaluation
            // we relax the rules
            //
            EnumScope = 1 << 9,

            ConstantScope = 1 << 10,

            ConstructorScope = 1 << 11,

            UsingInitializerScope = 1 << 12,

            LockScope = 1 << 13,

            TryScope = 1 << 14,

            TryWithCatchScope = 1 << 15,

            DontSetConditionalAccessReceiver = 1 << 16,

            ///
            /// Indicates the current context is in probing mode, no errors are reported. 
            ///
            ProbingMode = 1 << 22,

            //
            // Return and ContextualReturn statements will set the ReturnType
            // value based on the expression types of each return statement
            // instead of the method return type which is initially null.
            //
            InferReturnType = 1 << 23,

            OmitDebuggingInfo = 1 << 24,

            ExpressionTreeConversion = 1 << 25,

            InvokeSpecialName = 1 << 26,
            LoopScope = 1 << 27,
            SafeScope = 1 << 28,
            RestrictScope = 1 << 29
        }
        // utility helper for CheckExpr, UnCheckExpr, Checked and Unchecked statements
        // it's public so that we can use a struct at the callsite
        public struct FlagsHandle : IDisposable
        {
            readonly ResolveContext ec;
            readonly Options invmask, oldval;

            public FlagsHandle(ResolveContext ec, Options flagsToSet)
                : this(ec, flagsToSet, flagsToSet)
            {
            }

            internal FlagsHandle(ResolveContext ec, Options mask, Options val)
            {
                this.ec = ec;
                invmask = ~mask;
                oldval = ec.flags & mask;
                ec.flags = (ec.flags & invmask) | (val & mask);

                //				if ((mask & Options.ProbingMode) != 0)
                //					ec.Report.DisableReporting ();
            }

            public void Dispose()
            {
                //				if ((invmask & Options.ProbingMode) == 0)
                //					ec.Report.EnableReporting ();

                ec.flags = (ec.flags & invmask) | oldval;
            }
        }

        public static readonly string[][] names;
         static ResolveContext()
        {
            names = new string[(int)OperatorType.TOP][];
            names[(int)OperatorType.LogicalNot] = new string[] { "!", "op_LogicalNot" };
            names[(int)OperatorType.OnesComplement] = new string[] { "~", "op_OnesComplement" };
            names[(int)OperatorType.Increment] = new string[] { "++", "op_Increment" };
            names[(int)OperatorType.Decrement] = new string[] { "--", "op_Decrement" };
            names[(int)OperatorType.True] = new string[] { "true", "op_True" };
            names[(int)OperatorType.False] = new string[] { "false", "op_False" };
            names[(int)OperatorType.Addition] = new string[] { "+", "op_Addition" };
            names[(int)OperatorType.Subtraction] = new string[] { "-", "op_Subtraction" };
            names[(int)OperatorType.UnaryPlus] = new string[] { "+", "op_UnaryPlus" };
            names[(int)OperatorType.UnaryNegation] = new string[] { "-", "op_UnaryNegation" };
            names[(int)OperatorType.Multiply] = new string[] { "*", "op_Multiply" };
            names[(int)OperatorType.Division] = new string[] { "/", "op_Division" };
            names[(int)OperatorType.Modulus] = new string[] { "%", "op_Modulus" };
            names[(int)OperatorType.BitwiseAnd] = new string[] { "&", "op_BitwiseAnd" };
            names[(int)OperatorType.BitwiseOr] = new string[] { "|", "op_BitwiseOr" };
            names[(int)OperatorType.ExclusiveOr] = new string[] { "^", "op_ExclusiveOr" };
            names[(int)OperatorType.LeftShift] = new string[] { "<<", "op_LeftShift" };
            names[(int)OperatorType.RightShift] = new string[] { ">>", "op_RightShift" };
            names[(int)OperatorType.LeftRotate] = new string[] { "<~", "op_LeftRotate" };
            names[(int)OperatorType.RightRotate] = new string[] { "~>", "op_RightRotate" };
            names[(int)OperatorType.Equality] = new string[] { "==", "op_Equality" };
            names[(int)OperatorType.Inequality] = new string[] { "!=", "op_Inequality" };
            names[(int)OperatorType.GreaterThan] = new string[] { ">", "op_GreaterThan" };
            names[(int)OperatorType.LessThan] = new string[] { "<", "op_LessThan" };
            names[(int)OperatorType.GreaterThanOrEqual] = new string[] { ">=", "op_GreaterThanOrEqual" };
            names[(int)OperatorType.LessThanOrEqual] = new string[] { "<=", "op_LessThanOrEqual" };
            names[(int)OperatorType.Implicit] = new string[] { "implicit", "op_Implicit" };
            names[(int)OperatorType.Explicit] = new string[] { "explicit", "op_Explicit" };
            names[(int)OperatorType.Is] = new string[] { "is", "op_Is" };
            //names[(int)OperatorType.BinaryOperatorConstant] = new string[] { "@@", "op_B_" };
            //names[(int)OperatorType.UnaryOperatorConstant] = new string[] { "@", "op_U_" };
        }
        public static readonly Expression ErrorResult = ErrorExpression.UnknownError;
        public readonly ICompilation compilation;
		internal readonly VSharpConversions conversions;
		public readonly VSharpTypeResolveContext context;
		public readonly bool checkForOverflow;
		public readonly bool isWithinLambdaExpression;
        public readonly Report Report;
        protected Options flags;
        //
        // Whether we are inside an anonymous method.
        //
        public LambdaExpression CurrentAnonymousMethod;
        public bool HasSet(Options options)
        {
            return (this.flags & options) == options;
        }

        public bool HasAny(Options options)
        {
            return (this.flags & options) != 0;
        }
        public bool IsStatic
        {
            get
            {
             
                if (CurrentMember != null)
                    return CurrentMember.IsStatic;
                else if (CurrentTypeDefinition != null && CurrentMember == null)
                    return CurrentTypeDefinition.IsStatic;
                else return false;
            }
        }


        public bool IsStaticType(IType t)
        {
            if (t is ResolvedTypeDefinitionSpec)
                return (t as ResolvedTypeDefinitionSpec).IsStatic;
            else if (t is ElementTypeSpec)
                return IsStaticType((t as ElementTypeSpec).ElementType);
            else return false;
        }
        // Temporarily set all the given flags to the given value.  Should be used in an 'using' statement
        public FlagsHandle Set(Options options)
        {
            return new FlagsHandle(this, options);
        }

        public FlagsHandle With(Options options, bool enable)
        {
            return new FlagsHandle(this, options, enable ? options : 0);
        }
        #region Constructor
        public ResolveContext(ICompilation compilation, Report report=null)
		{
			if (compilation == null)
				throw new ArgumentNullException("compilation");
			this.compilation = compilation;
			this.conversions = VSharpConversions.Get(compilation);
			this.context = new VSharpTypeResolveContext(compilation.MainAssembly);
		    this.Report = report;
			var pc = compilation.MainAssembly.UnresolvedAssembly as VSharpProjectContent;
			if (pc != null) {
				this.checkForOverflow = pc.CompilerSettings.CheckForOverflow;
			}
		}

        public ResolveContext(VSharpTypeResolveContext context, Report report = null)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.compilation = context.Compilation;
			this.conversions = VSharpConversions.Get(compilation);
			this.context = context;
            this.Report = report ?? CompilerContext.report;
			if (context.CurrentTypeDefinition != null)
				currentTypeDefinitionCache = new TypeDefinitionCache(context.CurrentTypeDefinition);
		}
		
		private ResolveContext(ICompilation compilation, VSharpConversions conversions, VSharpTypeResolveContext context, bool checkForOverflow, bool isWithinLambdaExpression, TypeDefinitionCache currentTypeDefinitionCache, ImmutableStack<IVariable> localVariableStack, ObjectInitializerContext objectInitializerStack)
        {
            this.Report = CompilerContext.report;
			this.compilation = compilation;
			this.conversions = conversions;
			this.context = context;
			this.checkForOverflow = checkForOverflow;
			this.isWithinLambdaExpression = isWithinLambdaExpression;
			this.currentTypeDefinitionCache = currentTypeDefinitionCache;
			this.localVariableStack = localVariableStack;
			this.objectInitializerStack = objectInitializerStack;
		}
		#endregion
		
		#region Properties
		/// <summary>
		/// Gets the compilation used by the resolver.
		/// </summary>
		public ICompilation Compilation {
			get { return compilation; }
		}
		
		/// <summary>
		/// Gets the current type resolve context.
		/// </summary>
		public VSharpTypeResolveContext CurrentTypeResolveContext {
			get { return context; }
		}

		IAssembly ITypeResolveContext.CurrentAssembly {
			get { return context.CurrentAssembly; }
		}
		
		ResolveContext WithContext(VSharpTypeResolveContext newContext)
		{
            
			return new ResolveContext(compilation, conversions, newContext, checkForOverflow, isWithinLambdaExpression, currentTypeDefinitionCache, localVariableStack, objectInitializerStack);
		}
		
		/// <summary>
		/// Gets whether the current context is <c>checked</c>.
		/// </summary>
		public bool CheckForOverflow {
			get { return checkForOverflow; }
		}
		
		/// <summary>
		/// Sets whether the current context is <c>checked</c>.
		/// </summary>
		public ResolveContext WithCheckForOverflow(bool checkForOverflow)
		{
			if (checkForOverflow == this.checkForOverflow)
				return this;
			return new ResolveContext(compilation, conversions, context, checkForOverflow, isWithinLambdaExpression, currentTypeDefinitionCache, localVariableStack, objectInitializerStack);
		}
		
		/// <summary>
		/// Gets whether the resolver is currently within a lambda expression or anonymous method.
		/// </summary>
		public bool IsWithinLambdaExpression {
			get { return isWithinLambdaExpression; }
		}
		
		/// <summary>
		/// Sets whether the resolver is currently within a lambda expression.
		/// </summary>
		public ResolveContext WithIsWithinLambdaExpression(bool isWithinLambdaExpression)
		{
			return new ResolveContext(compilation, conversions, context, checkForOverflow, isWithinLambdaExpression, currentTypeDefinitionCache, localVariableStack, objectInitializerStack);
		}
		
		/// <summary>
		/// Gets the current member definition that is used to look up identifiers as parameters
		/// or type parameters.
		/// </summary>
		public IMember CurrentMember {
			get { return context.CurrentMember; }
		}
		
		/// <summary>
		/// Sets the current member definition.
		/// </summary>
		/// <remarks>Don't forget to also set CurrentTypeDefinition when setting CurrentMember;
		/// setting one of the properties does not automatically set the other.</remarks>
		public ResolveContext WithCurrentMember(IMember member)
		{
			return WithContext(context.WithCurrentMember(member));
		}
		
		ITypeResolveContext ITypeResolveContext.WithCurrentMember(IMember member)
		{
			return WithCurrentMember(member);
		}
		
		/// <summary>
		/// Gets the current using scope that is used to look up identifiers as class names.
		/// </summary>
		public ResolvedUsingScope CurrentUsingScope {
			get { return context.CurrentUsingScope; }
		}
		
		/// <summary>
		/// Sets the current using scope that is used to look up identifiers as class names.
		/// </summary>
		public ResolveContext WithCurrentUsingScope(ResolvedUsingScope usingScope)
		{
			return WithContext(context.WithUsingScope(usingScope));
		}
		#endregion
		
		#region Per-CurrentTypeDefinition Cache
		public readonly TypeDefinitionCache currentTypeDefinitionCache;
		
		/// <summary>
		/// Gets the current type definition.
		/// </summary>
		public ITypeDefinition CurrentTypeDefinition {
			get { return context.CurrentTypeDefinition; }
		}
		
		/// <summary>
		/// Sets the current type definition.
		/// </summary>
		public ResolveContext WithCurrentTypeDefinition(ITypeDefinition typeDefinition)
		{
			if (this.CurrentTypeDefinition == typeDefinition)
				return this;
			
			TypeDefinitionCache newTypeDefinitionCache;
			if (typeDefinition != null)
				newTypeDefinitionCache = new TypeDefinitionCache(typeDefinition);
			else
				newTypeDefinitionCache = null;
			
			return new ResolveContext(compilation, conversions, context.WithCurrentTypeDefinition(typeDefinition),
			                          checkForOverflow, isWithinLambdaExpression, newTypeDefinitionCache, localVariableStack, objectInitializerStack);
		}
		
		ITypeResolveContext ITypeResolveContext.WithCurrentTypeDefinition(ITypeDefinition typeDefinition)
		{
			return WithCurrentTypeDefinition(typeDefinition);
		}
		
		public sealed class TypeDefinitionCache
		{
			public readonly ITypeDefinition TypeDefinition;
            public readonly Dictionary<string, VSC.AST.Expression> SimpleNameLookupCacheExpression = new Dictionary<string, VSC.AST.Expression>();
            public readonly Dictionary<string, VSC.AST.Expression> SimpleNameLookupCacheInvocationTarget = new Dictionary<string, VSC.AST.Expression>();
            public readonly Dictionary<string, VSC.AST.Expression> SimpleTypeLookupCache = new Dictionary<string, VSC.AST.Expression>();
			
			public TypeDefinitionCache(ITypeDefinition typeDefinition)
			{
				this.TypeDefinition = typeDefinition;
			}
		}
		#endregion
		
		#region Local Variable Management
		
		// We store the local variables in an immutable stack.
		// The beginning of a block is marked by a null entry.
		
		// This data structure is used to allow efficient cloning of the resolver with its local variable context.
		readonly ImmutableStack<IVariable> localVariableStack = ImmutableStack<IVariable>.Empty;
		
		ResolveContext WithLocalVariableStack(ImmutableStack<IVariable> stack)
		{
			return new ResolveContext(compilation, conversions, context, checkForOverflow, isWithinLambdaExpression, currentTypeDefinitionCache, stack, objectInitializerStack);
		}
		
		/// <summary>
		/// Opens a new scope for local variables.
		/// </summary>
		public ResolveContext PushBlock()
		{
			return WithLocalVariableStack(localVariableStack.Push(null));
		}
		
		/// <summary>
		/// Closes the current scope for local variables; removing all variables in that scope.
		/// </summary>
		public ResolveContext PopBlock()
		{
			var stack = localVariableStack;
			IVariable removedVar;
			do {
				removedVar = stack.Peek();
				stack = stack.Pop();
			} while (removedVar != null);
			return WithLocalVariableStack(stack);
		}
		
		/// <summary>
		/// Adds a new variable or lambda parameter to the current block.
		/// </summary>
		public ResolveContext AddVariable(IVariable variable)
		{
			if (variable == null)
				throw new ArgumentNullException("variable");
			return WithLocalVariableStack(localVariableStack.Push(variable));
		}
		
		/// <summary>
		/// Removes the variable that was just added.
		/// </summary>
		public ResolveContext PopLastVariable()
		{
			if (localVariableStack.Peek() == null)
				throw new InvalidOperationException("There is no variable within the current block.");
			return WithLocalVariableStack(localVariableStack.Pop());
		}
		
		/// <summary>
		/// Gets all currently visible local variables and lambda parameters.
		/// Does not include method parameters.
		/// </summary>
		public IEnumerable<IVariable> LocalVariables {
			get {
				return localVariableStack.Where(v => v != null);
			}
		}
		#endregion
		
		#region Object Initializer Context
		sealed class ObjectInitializerContext
		{
            internal readonly Expression initializedObject;
			internal readonly ObjectInitializerContext prev;
			
			public ObjectInitializerContext(Expression initializedObject, ResolveContext.ObjectInitializerContext prev)
			{
				this.initializedObject = initializedObject;
				this.prev = prev;
			}
		}
		
		readonly ObjectInitializerContext objectInitializerStack;
		
		ResolveContext WithObjectInitializerStack(ObjectInitializerContext stack)
		{
			return new ResolveContext(compilation, conversions, context, checkForOverflow, isWithinLambdaExpression, currentTypeDefinitionCache, localVariableStack, stack);
		}
		
		/// <summary>
		/// Pushes the type of the object that is currently being initialized.
		/// </summary>
		public ResolveContext PushObjectInitializer(Expression initializedObject)
		{
			if (initializedObject == null)
				throw new ArgumentNullException("initializedObject");
			return WithObjectInitializerStack(new ObjectInitializerContext(initializedObject, objectInitializerStack));
		}
		
		public ResolveContext PopObjectInitializer()
		{
			if (objectInitializerStack == null)
				throw new InvalidOperationException();
			return WithObjectInitializerStack(objectInitializerStack.prev);
		}
		
		/// <summary>
		/// Gets whether this context is within an object initializer.
		/// </summary>
		public bool IsInObjectInitializer {
			get { return objectInitializerStack != null; }
		}
		
		/// <summary>
		/// Gets the current object initializer. This usually is an <see cref="InitializedObjectExpression"/>
		/// or (for nested initializers) a semantic tree based on an <see cref="InitializedObjectExpression"/>.
		/// Returns ErrorResolveResult if there is no object initializer.
		/// </summary>
		public VSC.AST.Expression CurrentObjectInitializer {
			get {
				return objectInitializerStack != null ? objectInitializerStack.initializedObject : ErrorResult;
			}
		}
		
		/// <summary>
		/// Gets the type of the object currently being initialized.
		/// Returns SharedTypes.Unknown if no object initializer is currently open (or if the object initializer
		/// has unknown type).
		/// </summary>
		public IType CurrentObjectInitializerType {
			get { return CurrentObjectInitializer.Type; }
		}
		#endregion
		
		#region Clone
		/// <summary>
		/// Creates a copy of this VSharp resolver.
		/// </summary>
		[Obsolete("ResolveContext is immutable, cloning is no longer necessary")]
		public ResolveContext Clone()
		{
			return this;
		}
		#endregion
    
        public static ExpressionType GetLinqNodeType(BinaryOperatorType op, bool checkForOverflow)
        {
            switch (op)
            {
                case BinaryOperatorType.BitwiseAnd:
                    return ExpressionType.And;
                case BinaryOperatorType.BitwiseOr:
                    return ExpressionType.Or;
                case BinaryOperatorType.LogicalAnd:
                    return ExpressionType.AndAlso;
                case BinaryOperatorType.LogicalOr:
                    return ExpressionType.OrElse;
                case BinaryOperatorType.ExclusiveOr:
                    return ExpressionType.ExclusiveOr;
                case BinaryOperatorType.GreaterThan:
                    return ExpressionType.GreaterThan;
                case BinaryOperatorType.GreaterThanOrEqual:
                    return ExpressionType.GreaterThanOrEqual;
                case BinaryOperatorType.Equality:
                    return ExpressionType.Equal;
                case BinaryOperatorType.Inequality:
                    return ExpressionType.NotEqual;
                case BinaryOperatorType.LessThan:
                    return ExpressionType.LessThan;
                case BinaryOperatorType.LessThanOrEqual:
                    return ExpressionType.LessThanOrEqual;
                case BinaryOperatorType.Addition:
                    return checkForOverflow ? ExpressionType.AddChecked : ExpressionType.Add;
                case BinaryOperatorType.Subtraction:
                    return checkForOverflow ? ExpressionType.SubtractChecked : ExpressionType.Subtract;
                case BinaryOperatorType.Multiply:
                    return checkForOverflow ? ExpressionType.MultiplyChecked : ExpressionType.Multiply;
                case BinaryOperatorType.Division:
                    return ExpressionType.Divide;
                case BinaryOperatorType.Modulus:
                    return ExpressionType.Modulo;
                case BinaryOperatorType.LeftShift:
                    return ExpressionType.LeftShift;
                case BinaryOperatorType.RightShift:
                    return ExpressionType.RightShift;

                // special implementation
                case BinaryOperatorType.RotateLeft:
                    return ExpressionType.LeftShift;
                case BinaryOperatorType.RotateRight:
                    return ExpressionType.RightShift;
                case BinaryOperatorType.Is:
                    return ExpressionType.TypeIs;

                case BinaryOperatorType.NullCoalescing:
                    return ExpressionType.Coalesce;
                default:
                    throw new NotSupportedException("Invalid value for BinaryOperatorType");
            }
        }
        public static ExpressionType GetLinqNodeType(AssignmentOperatorType op, bool checkForOverflow)
        {
            switch (op)
            {
                case AssignmentOperatorType.Assign:
                    return ExpressionType.Assign;
                case AssignmentOperatorType.Add:
                    return checkForOverflow ? ExpressionType.AddAssignChecked : ExpressionType.AddAssign;
                case AssignmentOperatorType.Subtract:
                    return checkForOverflow ? ExpressionType.SubtractAssignChecked : ExpressionType.SubtractAssign;
                case AssignmentOperatorType.Multiply:
                    return checkForOverflow ? ExpressionType.MultiplyAssignChecked : ExpressionType.MultiplyAssign;
                case AssignmentOperatorType.Divide:
                    return ExpressionType.DivideAssign;
                case AssignmentOperatorType.Modulus:
                    return ExpressionType.ModuloAssign;
                case AssignmentOperatorType.ShiftLeft:
                    return ExpressionType.LeftShiftAssign;
                case AssignmentOperatorType.ShiftRight:
                    return ExpressionType.RightShiftAssign;
                case AssignmentOperatorType.BitwiseAnd:
                    return ExpressionType.AndAssign;
                case AssignmentOperatorType.BitwiseOr:
                    return ExpressionType.OrAssign;
                case AssignmentOperatorType.ExclusiveOr:
                    return ExpressionType.ExclusiveOrAssign;
                default:
                    throw new NotSupportedException("Invalid value for AssignmentOperatorType");
            }
        }
        public static ExpressionType GetLinqNodeType(UnaryOperatorType op, bool checkForOverflow)
        {
            switch (op)
            {
                case UnaryOperatorType.LogicalNot:
                    return ExpressionType.Not;
                case UnaryOperatorType.OnesComplement:
                    return ExpressionType.OnesComplement;
                case UnaryOperatorType.UnaryNegation:
                    return checkForOverflow ? ExpressionType.NegateChecked : ExpressionType.Negate;
                case UnaryOperatorType.UnaryPlus:
                    return ExpressionType.UnaryPlus;
                case UnaryOperatorType.PreIncrement:
                    return ExpressionType.PreIncrementAssign;
                case UnaryOperatorType.Decrement:
                    return ExpressionType.PreDecrementAssign;
                case UnaryOperatorType.PostIncrement:
                    return ExpressionType.PostIncrementAssign;
                case UnaryOperatorType.PostDecrement:
                    return ExpressionType.PostDecrementAssign;
                case UnaryOperatorType.Dereference:
                case UnaryOperatorType.AddressOf:
                    //  case UnaryOperatorType.UnaryOperator:
                    return ExpressionType.Extension;
                default:
                    throw new NotSupportedException("Invalid value for UnaryOperatorType");
            }
        }

		
		
		
		
		#region Enum helper methods
	public static	IType GetEnumUnderlyingType(IType enumType)
		{
			ITypeDefinition def = enumType.GetDefinition();
			return def != null ? def.EnumUnderlyingType : SpecialTypeSpec.UnknownType;
		}
		
		#region GetOverloadableOperatorName
	internal	static string GetOverloadableOperatorName(BinaryOperatorType op)
		{
			switch (op) {
				case BinaryOperatorType.Addition:
					return "op_Addition";
				case BinaryOperatorType.Subtraction:
					return "op_Subtraction";
				case BinaryOperatorType.Multiply:
					return "op_Multiply";
				case BinaryOperatorType.Division:
					return "op_Division";
				case BinaryOperatorType.Modulus:
					return "op_Modulus";
				case BinaryOperatorType.BitwiseAnd:
					return "op_BitwiseAnd";
				case BinaryOperatorType.BitwiseOr:
					return "op_BitwiseOr";
				case BinaryOperatorType.ExclusiveOr:
					return "op_ExclusiveOr";
				case BinaryOperatorType.LeftShift:
					return "op_LeftShift";
				case BinaryOperatorType.RightShift:
					return "op_RightShift";
                case BinaryOperatorType.RotateLeft:
                    return "op_LeftRotate";
                case BinaryOperatorType.RotateRight:
                    return "op_RightRotate";
                case BinaryOperatorType.Is:
                    return "op_Is";
				case BinaryOperatorType.Equality:
					return "op_Equality";
				case BinaryOperatorType.Inequality:
					return "op_Inequality";
				case BinaryOperatorType.GreaterThan:
					return "op_GreaterThan";
				case BinaryOperatorType.LessThan:
					return "op_LessThan";
				case BinaryOperatorType.GreaterThanOrEqual:
					return "op_GreaterThanOrEqual";
				case BinaryOperatorType.LessThanOrEqual:
					return "op_LessThanOrEqual";
				default:
					return null;
			}
		}
		#endregion
		
		#endregion
		
		#region Get user-defined operator candidates
	internal	IEnumerable<IParameterizedMember> GetUserDefinedOperatorCandidates(IType type, string operatorName)
		{
			if (operatorName == null)
				return EmptyList<IMethod>.Instance;
			TypeCode c = ReflectionHelper.GetTypeCode(type);
			if (TypeCode.Boolean <= c && c <= TypeCode.Decimal || c == TypeCode.String) {
				// The .NET framework contains some of V#'s built-in operators as user-defined operators.
				// However, we must not use those as user-defined operators (we would skip numeric promotion).
				return EmptyList<IMethod>.Instance;
			}
			// V# 4.0 spec: §7.3.5 Candidate user-defined operators
			var operators = type.GetMethods(m => m.IsOperator && m.Name == operatorName).ToList();
			LiftUserDefinedOperators(operators);
			return operators;
		}
		
		void LiftUserDefinedOperators(List<IMethod> operators)
		{
			int nonLiftedMethodCount = operators.Count;
			// Construct lifted operators
			for (int i = 0; i < nonLiftedMethodCount; i++) {
				var liftedMethod = LiftUserDefinedOperator(operators[i]);
				if (liftedMethod != null)
					operators.Add(liftedMethod);
			}
		}
		
		LiftedUserDefinedOperator LiftUserDefinedOperator(IMethod m)
		{
			if (IsComparisonOperator(m)) {
				if (!m.ReturnType.Equals(compilation.FindType(KnownTypeCode.Boolean)))
					return null; // cannot lift this operator
			} else {
				if (!NullableType.IsNonNullableValueType(m.ReturnType))
					return null; // cannot lift this operator
			}
			for (int i = 0; i < m.Parameters.Count; i++) {
				if (!NullableType.IsNonNullableValueType(m.Parameters[i].Type))
					return null; // cannot lift this operator
			}
			return new LiftedUserDefinedOperator(m);
		}
        public static OperatorType? GetOperatorType(string methodName)
        {
            for (int i = 0; i < names.Length; ++i)
            {
                if (names[i][1] == methodName)
                    return (OperatorType?)i;
            }
            return null;

        }
        public static string GetMetadataName(OperatorType ot)
        {
            return names[(int)ot][1];
        }
        public static string GetMetadataName(string opsymbol, ref OperatorType opt)
        {
            for (int i = 0; i < names.Length; ++i)
            {

                if (names[i][0] == opsymbol)
                {
                    opt = (OperatorType)i;
                    return names[i][1];
                }
            }
            opt = OperatorType.TOP;
            return null;
        }
		static bool IsComparisonOperator(IMethod m)
		{
			var type = GetOperatorType(m.Name);
			return type.HasValue && type.Value.IsComparisonOperator();
		}
		
		sealed class LiftedUserDefinedOperator : SpecializedMethod, OverloadResolution.ILiftedOperator
		{
			internal readonly IParameterizedMember nonLiftedOperator;
			
			public LiftedUserDefinedOperator(IMethod nonLiftedMethod)
				: base((IMethod)nonLiftedMethod.MemberDefinition, nonLiftedMethod.Substitution)
			{
				this.nonLiftedOperator = nonLiftedMethod;
				var substitution = new MakeNullableVisitor(nonLiftedMethod.Compilation, nonLiftedMethod.Substitution);
				this.Parameters = base.CreateParameters(substitution);
				// Comparison operators keep the 'bool' return type even when lifted.
				if (IsComparisonOperator(nonLiftedMethod))
					this.ReturnType = nonLiftedMethod.ReturnType;
				else
					this.ReturnType = nonLiftedMethod.ReturnType.AcceptVisitor(substitution);
			}
			
			public IList<IParameter> NonLiftedParameters {
				get { return nonLiftedOperator.Parameters; }
			}
			
			public override bool Equals(object obj)
			{
				LiftedUserDefinedOperator op = obj as LiftedUserDefinedOperator;
				return op != null && this.nonLiftedOperator.Equals(op.nonLiftedOperator);
			}
			
			public override int GetHashCode()
			{
				return nonLiftedOperator.GetHashCode() ^ 0x7191254;
			}
		}
		
		sealed class MakeNullableVisitor : TypeVisitor
		{
			readonly ICompilation compilation;
			readonly TypeParameterSubstitution typeParameterSubstitution;
			
			public MakeNullableVisitor(ICompilation compilation, TypeParameterSubstitution typeParameterSubstitution)
			{
				this.compilation = compilation;
				this.typeParameterSubstitution = typeParameterSubstitution;
			}
			
			public override IType VisitTypeDefinition(ITypeDefinition type)
			{
				return NullableType.Create(compilation, type.AcceptVisitor(typeParameterSubstitution));
			}
			
			public override IType VisitTypeParameter(ITypeParameter type)
			{
				return NullableType.Create(compilation, type.AcceptVisitor(typeParameterSubstitution));
			}
			
			public override IType VisitParameterizedType(ParameterizedTypeSpec type)
			{
				return NullableType.Create(compilation, type.AcceptVisitor(typeParameterSubstitution));
			}
			
			public override IType VisitOtherType(IType type)
			{
				return NullableType.Create(compilation, type.AcceptVisitor(typeParameterSubstitution));
			}
		}
		#endregion
		
		#region ResolveCast
		
        internal bool TryConvert(ref Expression rr, IType targetType)
        {
            Conversion c = conversions.ImplicitConversion(rr, targetType);
            if (c.IsValid)
            {
                rr = Convert(rr, targetType, c);
                return true;
            }
            else
            {
                return false;
            }
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="rr">The input resolve result that should be converted.
		/// If a conversion exists, it is applied to the resolve result</param>
		/// <param name="targetType">The target type that we should convert to</param>
		/// <param name="isNullable">Whether we are dealing with a lifted operator</param>
		/// <param name="enumRR">The resolve result that is enum-typed.
		/// If necessary, a nullable conversion is applied.</param>
		/// <param name="allowConversionFromConstantZero">
		/// Whether the conversion from the constant zero is allowed.
		/// </param>
		/// <returns>True if the conversion is successful; false otherwise.
		/// If the conversion is not successful, the ref parameters will not be modified.</returns>
	internal	bool TryConvertEnum(ref Expression rr, IType targetType, ref bool isNullable, ref Expression enumRR, bool allowConversionFromConstantZero = true)
		{
			Conversion c;
			if (!isNullable) {
				// Try non-nullable
				c = conversions.ImplicitConversion(rr, targetType);
				if (c.IsValid && (allowConversionFromConstantZero || !c.IsEnumerationConversion)) {
					rr = Convert(rr, targetType, c);
					return true;
				}
			}
			// make targetType nullable if it isn't already:
			if (!targetType.IsKnownType(KnownTypeCode.NullableOfT))
				targetType = NullableType.Create(compilation, targetType);
			
			c = conversions.ImplicitConversion(rr, targetType);
			if (c.IsValid && (allowConversionFromConstantZero || !c.IsEnumerationConversion)) {
				rr = Convert(rr, targetType, c);
				isNullable = true;
				// Also convert the enum-typed RR to nullable, if it isn't already
				if (!enumRR.Type.IsKnownType(KnownTypeCode.NullableOfT)) {
					var nullableType = NullableType.Create(compilation, enumRR.Type);
					enumRR = new CastExpression(nullableType, enumRR, Conversion.ImplicitNullableConversion);
				}
				return true;
			}
			return false;
		}

        internal Expression Convert(Expression rr, IType targetType)
		{
			return Convert(rr, targetType, conversions.ImplicitConversion(rr, targetType));
		}
		
	internal	Expression Convert(Expression rr, IType targetType, Conversion c)
		{
			if (c == Conversion.IdentityConversion)
				return rr;
			else if (rr.IsCompileTimeConstant && c != Conversion.None && !c.IsUserDefined)
                return new CastExpression(targetType, rr).DoResolve(this);
			else
				return new CastExpression(targetType, rr, c, checkForOverflow);
		}

        internal object VSharpPrimitiveCast(TypeCode targetType, object input)
		{
			return Base.VSharpPrimitiveCast.Cast(targetType, input, this.CheckForOverflow);
		}
		#endregion

        
        #region ResolveSimpleName
        public bool IsVariableReferenceWithSameType(Expression rr,out TypeExpression trr)
        {
            if (!(rr is MemberExpressionStatement || rr is LocalVariableExpression))
            {
                trr = null;
                return false;
            }
            rr.lookupMode = NameLookupMode.Type;
            trr = rr.DoResolve(this) as TypeExpression; 
            return trr != null && trr.Type.Equals(rr.Type);
        }

       #endregion
		
		#region ResolveMemberAccess	
		/// <summary>
		/// Creates a MemberLookup instance using this resolver's settings.
		/// </summary>
		public MemberLookup CreateMemberLookup()
		{
			ITypeDefinition currentTypeDefinition = this.CurrentTypeDefinition;
			bool isInEnumMemberInitializer = this.CurrentMember != null && this.CurrentMember.SymbolKind == SymbolKind.Field
				&& currentTypeDefinition != null && currentTypeDefinition.Kind == TypeKind.Enum;
			return new MemberLookup(currentTypeDefinition, this.Compilation.MainAssembly, isInEnumMemberInitializer);
		}
		
		/// <summary>
		/// Creates a MemberLookup instance using this resolver's settings.
		/// </summary>
		public MemberLookup CreateMemberLookup(NameLookupMode lookupMode)
		{
			if (lookupMode == NameLookupMode.BaseTypeReference && this.CurrentTypeDefinition != null) {
				// When looking up a base type reference, treat us as being outside the current type definition
				// for accessibility purposes.
				// This avoids a stack overflow when referencing a protected class nested inside the base class
				// of a parent class. (NameLookupTests.InnerClassInheritingFromProtectedBaseInnerClassShouldNotCauseStackOverflow)
				return new MemberLookup(this.CurrentTypeDefinition.DeclaringTypeDefinition, this.Compilation.MainAssembly, false);
			} else {
				return CreateMemberLookup();
			}
		}
		#endregion
		
		#region ResolveIdentifierInObjectInitializer
        public VSC.AST.Expression ResolveIdentifierInObjectInitializer(string identifier)
		{
			MemberLookup memberLookup = CreateMemberLookup();
			return memberLookup.Lookup(this.CurrentObjectInitializer, identifier, EmptyList<IType>.Instance, false);
		}
		#endregion
		
		#region GetExtensionMethods
		/// <summary>
		/// Gets all extension methods that are available in the current context.
		/// </summary>
		/// <param name="name">Name of the extension method. Pass null to retrieve all extension methods.</param>
		/// <param name="typeArguments">Explicitly provided type arguments.
		/// An empty list will return all matching extension method definitions;
		/// a non-empty list will return <see cref="SpecializedMethod"/>s for all extension methods
		/// with the matching number of type parameters.</param>
		/// <remarks>
		/// The results are stored in nested lists because they are grouped by using scope.
		/// That is, for "using SomeExtensions; namespace X { using MoreExtensions; ... }",
		/// the return value will be
		/// new List {
		///    new List { all extensions from MoreExtensions },
		///    new List { all extensions from SomeExtensions }
		/// }
		/// </remarks>
		public List<List<IMethod>> GetExtensionMethods(string name = null, IList<IType> typeArguments = null)
		{
			return GetExtensionMethods(null, name, typeArguments);
		}
		
		/// <summary>
		/// Gets the extension methods that are called 'name'
		/// and are applicable with a first argument type of 'targetType'.
		/// </summary>
		/// <param name="targetType">Type of the 'this' argument</param>
		/// <param name="name">Name of the extension method. Pass null to retrieve all extension methods.</param>
		/// <param name="typeArguments">Explicitly provided type arguments.
		/// An empty list will return all matching extension method definitions;
		/// a non-empty list will return <see cref="SpecializedMethod"/>s for all extension methods
		/// with the matching number of type parameters.</param>
		/// <param name="substituteInferredTypes">
		/// Specifies whether to produce a <see cref="SpecializedMethod"/>
		/// when type arguments could be inferred from <paramref name="targetType"/>. This parameter
		/// is only used for inferred types and has no effect if <paramref name="typeArguments"/> is non-empty.
		/// </param>
		/// <remarks>
		/// The results are stored in nested lists because they are grouped by using scope.
		/// That is, for "using SomeExtensions; namespace X { using MoreExtensions; ... }",
		/// the return value will be
		/// new List {
		///    new List { all extensions from MoreExtensions },
		///    new List { all extensions from SomeExtensions }
		/// }
		/// </remarks>
		public List<List<IMethod>> GetExtensionMethods(IType targetType, string name = null, IList<IType> typeArguments = null, bool substituteInferredTypes = false)
		{
			var lookup = CreateMemberLookup();
			List<List<IMethod>> extensionMethodGroups = new List<List<IMethod>>();
			foreach (var inputGroup in GetAllExtensionMethods(lookup)) {
				List<IMethod> outputGroup = new List<IMethod>();
				foreach (var method in inputGroup) {
					if (name != null && method.Name != name)
						continue;
					if (!lookup.IsAccessible(method, false))
						continue;
					IType[] inferredTypes;
					if (typeArguments != null && typeArguments.Count > 0) {
						if (method.TypeParameters.Count != typeArguments.Count)
							continue;
						var sm = method.Specialize(new TypeParameterSubstitution(null, typeArguments));
						if (IsEligibleExtensionMethod(compilation, conversions, targetType, sm, false, out inferredTypes))
							outputGroup.Add(sm);
					} else {
						if (IsEligibleExtensionMethod(compilation, conversions, targetType, method, true, out inferredTypes)) {
							if (substituteInferredTypes && inferredTypes != null) {
								outputGroup.Add(method.Specialize(new TypeParameterSubstitution(null, inferredTypes)));
							} else {
								outputGroup.Add(method);
							}
						}
					}
				}
				if (outputGroup.Count > 0)
					extensionMethodGroups.Add(outputGroup);
			}
			return extensionMethodGroups;
		}
		
		/// <summary>
		/// Checks whether the specified extension method is eligible on the target type.
		/// </summary>
		/// <param name="targetType">Target type that is passed as first argument to the extension method.</param>
		/// <param name="method">The extension method.</param>
		/// <param name="useTypeInference">Whether to perform type inference for the method.
		/// Use <c>false</c> if <paramref name="method"/> is already parameterized (e.g. when type arguments were given explicitly).
		/// Otherwise, use <c>true</c>.
		/// </param>
		/// <param name="outInferredTypes">If the method is generic and <paramref name="useTypeInference"/> is <c>true</c>,
		/// and at least some of the type arguments could be inferred, this parameter receives the inferred type arguments.
		/// Since only the type for the first parameter is considered, not all type arguments may be inferred.
		/// If an array is returned, any slot with an uninferred type argument will be set to the method's
		/// corresponding type parameter.
		/// </param>
		public static bool IsEligibleExtensionMethod(IType targetType, IMethod method, bool useTypeInference, out IType[] outInferredTypes)
		{
			if (targetType == null)
				throw new ArgumentNullException("targetType");
			if (method == null)
				throw new ArgumentNullException("method");
			var compilation = method.Compilation;
			return IsEligibleExtensionMethod(compilation, VSharpConversions.Get(compilation), targetType, method, useTypeInference, out outInferredTypes);
		}
		
		static bool IsEligibleExtensionMethod(ICompilation compilation, VSharpConversions conversions, IType targetType, IMethod method, bool useTypeInference, out IType[] outInferredTypes)
		{
			outInferredTypes = null;
			if (targetType == null)
				return true;
			if (method.Parameters.Count == 0)
				return false;
			IType thisParameterType = method.Parameters[0].Type;
			if (useTypeInference && method.TypeParameters.Count > 0) {
				// We need to infer type arguments from targetType:
				TypeInference ti = new TypeInference(compilation, conversions);
				Expression[] arguments = { new Expression(targetType) };
				IType[] parameterTypes = { method.Parameters[0].Type };
				bool success;
				var inferredTypes = ti.InferTypeArguments(method.TypeParameters, arguments, parameterTypes, out success);
				var substitution = new TypeParameterSubstitution(null, inferredTypes);
				// Validate that the types that could be inferred (aren't unknown) satisfy the constraints:
				bool hasInferredTypes = false;
				for (int i = 0; i < inferredTypes.Length; i++) {
					if (inferredTypes[i].Kind != TypeKind.Unknown && inferredTypes[i].Kind != TypeKind.UnboundTypeArgument) {
						hasInferredTypes = true;
						if (!OverloadResolution.ValidateConstraints(method.TypeParameters[i], inferredTypes[i], substitution, conversions))
							return false;
					} else {
						inferredTypes[i] = method.TypeParameters[i]; // do not substitute types that could not be inferred
					}
				}
				if (hasInferredTypes)
					outInferredTypes = inferredTypes;
				thisParameterType = thisParameterType.AcceptVisitor(substitution);
			}
			Conversion c = conversions.ImplicitConversion(targetType, thisParameterType);
			return c.IsValid && (c.IsIdentityConversion || c.IsReferenceConversion || c.IsBoxingConversion);
		}
		
		/// <summary>
		/// Gets all extension methods available in the current using scope.
		/// This list includes inaccessible methods.
		/// </summary>
		IList<List<IMethod>> GetAllExtensionMethods(MemberLookup lookup)
		{
			var currentUsingScope = context.CurrentUsingScope;
			if (currentUsingScope == null)
				return EmptyList<List<IMethod>>.Instance;
			List<List<IMethod>> extensionMethodGroups = LazyInit.VolatileRead(ref currentUsingScope.AllExtensionMethods);
			if (extensionMethodGroups != null) {
				return extensionMethodGroups;
			}
			extensionMethodGroups = new List<List<IMethod>>();
			List<IMethod> m;
			for (ResolvedUsingScope scope = currentUsingScope; scope != null; scope = scope.Parent) {
				INamespace ns = scope.Namespace;
				if (ns != null) {
					m = GetExtensionMethods(lookup, ns).ToList();
					if (m.Count > 0)
						extensionMethodGroups.Add(m);
				}
				
				m = scope.Usings
					.Distinct()
					.SelectMany(importedNamespace =>  GetExtensionMethods(lookup, importedNamespace))
					.ToList();
				if (m.Count > 0)
					extensionMethodGroups.Add(m);
			}
			return LazyInit.GetOrSet(ref currentUsingScope.AllExtensionMethods, extensionMethodGroups);
		}
		
		IEnumerable<IMethod> GetExtensionMethods(MemberLookup lookup, INamespace ns)
		{
			// TODO: maybe make this a property on INamespace?
			return
				from c in ns.Types
				where c.IsStatic && c.HasExtensionMethods && c.TypeParameters.Count == 0 && lookup.IsAccessible(c, false)
				from m in c.Methods
				where m.IsExtensionMethod
				select m;
		}
		#endregion
		
		#region ResolveInvocation


        internal IList<Expression> AddArgumentNamesIfNecessary(Expression[] arguments, string[] argumentNames)
        {
            if (argumentNames == null)
            {
                return arguments;
            }
            else
            {
                var result = new Expression[arguments.Length];
                for (int i = 0; i < arguments.Length; i++)
                {
                    result[i] = (argumentNames[i] != null ? new NamedArgumentExpression(argumentNames[i], arguments[i]) : arguments[i]);
                }
                return result;
            }
        }
        internal List<IParameter> CreateParameters(Expression[] arguments, string[] argumentNames)
		{
			List<IParameter> list = new List<IParameter>();
			if (argumentNames == null) {
				argumentNames = new string[arguments.Length];
			} else {
				if (argumentNames.Length != arguments.Length)
					throw new ArgumentException();
				argumentNames = (string[])argumentNames.Clone();
			}
			for (int i = 0; i < arguments.Length; i++) {
				// invent argument names where necessary:
				if (argumentNames[i] == null) {
					string newArgumentName = GuessParameterName(arguments[i]);
					if (argumentNames.Contains(newArgumentName)) {
						// disambiguate argument name (e.g. add a number)
						int num = 1;
						string newName;
						do {
							newName = newArgumentName + num.ToString();
							num++;
						} while(argumentNames.Contains(newName));
						newArgumentName = newName;
					}
					argumentNames[i] = newArgumentName;
				}
				
				// create the parameter:
				ByReferenceExpression brrr = arguments[i] as ByReferenceExpression;
				if (brrr != null) {
					list.Add(new ParameterSpec(arguments[i].Type, argumentNames[i], isRef: brrr.IsRef, isOut: brrr.IsOut));
				} else {
					// argument might be a lambda or delegate type, so we have to try to guess the delegate type
					IType type = arguments[i].Type;
					if (type.Kind == TypeKind.Null || type.Kind == TypeKind.Unknown) {
                        list.Add(new ParameterSpec(compilation.FindType(KnownTypeCode.Object), argumentNames[i]));
					} else {
                        list.Add(new ParameterSpec(type, argumentNames[i]));
					}
				}
			}
			return list;
		}
		internal static string GuessParameterName(Expression rr)
		{
            MemberExpressionStatement mrr = rr as MemberExpressionStatement;
			if (mrr != null)
				return mrr.Member.Name;
			
			UnknownMemberExpression umrr = rr as UnknownMemberExpression;
			if (umrr != null)
				return umrr.MemberName;

            MethodGroupExpression mgrr = rr as MethodGroupExpression;
			if (mgrr != null)
				return mgrr.MethodName;

            LocalVariableExpression vrr = rr as LocalVariableExpression;
			if (vrr != null)
				return MakeParameterName(vrr.Variable.Name);
			
			if (rr.Type.Kind != TypeKind.Unknown && !string.IsNullOrEmpty(rr.Type.Name)) {
				return MakeParameterName(rr.Type.Name);
			} else {
				return "parameter";
			}
		}
		internal static string MakeParameterName(string variableName)
		{
			if (string.IsNullOrEmpty(variableName))
				return "parameter";
			if (variableName.Length > 1 && variableName[0] == '_')
				variableName = variableName.Substring(1);
			return char.ToLower(variableName[0]) + variableName.Substring(1);
		}	
		internal OverloadResolution CreateOverloadResolution(Expression[] arguments, string[] argumentNames = null, IType[] typeArguments = null)
		{
			var or = new OverloadResolution(compilation, arguments, argumentNames, typeArguments, conversions);
			or.CheckForOverflow = checkForOverflow;
			return or;
		}
		#endregion
		
	
		
		#region ResolveConditional
		/// <summary>
		/// Converts the input to <c>bool</c> using the rules for boolean expressions.
		/// That is, <c>operator true</c> is used if a regular conversion to <c>bool</c> is not possible.
		/// </summary>
		public Expression ResolveCondition(Expression input)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			IType boolean = compilation.FindType(KnownTypeCode.Boolean);
			Conversion c = conversions.ImplicitConversion(input, boolean);
			if (!c.IsValid) {
				var opTrue = input.Type.GetMethods(m => m.IsOperator && m.Name == "op_True").FirstOrDefault();
				if (opTrue != null) {
					c = Conversion.UserDefinedConversion(opTrue, isImplicit: true, conversionBeforeUserDefinedOperator: Conversion.None, conversionAfterUserDefinedOperator: Conversion.None);
				}
			}
			return Convert(input, boolean, c);
		}
		
		/// <summary>
		/// Converts the negated input to <c>bool</c> using the rules for boolean expressions.
		/// Computes <c>!(bool)input</c> if the implicit cast to bool is valid; otherwise
		/// computes <c>input.operator false()</c>.
		/// </summary>
		public Expression ResolveConditionFalse(Expression input)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			IType boolean = compilation.FindType(KnownTypeCode.Boolean);
			Conversion c = conversions.ImplicitConversion(input, boolean);
			if (!c.IsValid) {
				var opFalse = input.Type.GetMethods(m => m.IsOperator && m.Name == "op_False").FirstOrDefault();
				if (opFalse != null) {
					c = Conversion.UserDefinedConversion(opFalse, isImplicit: true, conversionBeforeUserDefinedOperator: Conversion.None, conversionAfterUserDefinedOperator: Conversion.None);
					return Convert(input, boolean, c);
				}
			}
			return new VSC.AST.UnaryExpression(UnaryOperatorType.LogicalNot, Convert(input, boolean, c), input.Location);
		}
		

        internal bool IsBetterConditionalConversion(Conversion c1, Conversion c2)
		{
			// Valid is better than ImplicitConstantExpressionConversion is better than invalid
			if (!c1.IsValid)
				return false;
			if (c1 != Conversion.ImplicitConstantExpressionConversion && c2 == Conversion.ImplicitConstantExpressionConversion)
				return true;
			return !c2.IsValid;
		}
		
	internal	bool HasType(Expression r)
		{
			return r.Type.Kind != TypeKind.Unknown && r.Type.Kind != TypeKind.Null;
		}
		#endregion
		
		#region ResolveDefaultValue
	
		public static object GetDefaultValue(IType type)
		{
			ITypeDefinition typeDef = type.GetDefinition();
			if (typeDef == null)
				return null;
			if (typeDef.Kind == TypeKind.Enum) {
				typeDef = typeDef.EnumUnderlyingType.GetDefinition();
				if (typeDef == null)
					return null;
			}
			switch (typeDef.KnownTypeCode) {
				case KnownTypeCode.Boolean:
					return false;
				case KnownTypeCode.Char:
					return '\0';
				case KnownTypeCode.SByte:
					return (sbyte)0;
				case KnownTypeCode.Byte:
					return (byte)0;
				case KnownTypeCode.Int16:
					return (short)0;
				case KnownTypeCode.UInt16:
					return (ushort)0;
				case KnownTypeCode.Int32:
					return 0;
				case KnownTypeCode.UInt32:
					return 0U;
				case KnownTypeCode.Int64:
					return 0L;
				case KnownTypeCode.UInt64:
					return 0UL;
				case KnownTypeCode.Single:
					return 0f;
				case KnownTypeCode.Double:
					return 0.0;
				case KnownTypeCode.Decimal:
					return 0m;
				default:
					return null;
			}
		}
		#endregion
		
		#region ResolveArrayCreation
		/// <summary>
		/// Resolves an array creation.
		/// </summary>
		/// <param name="elementType">
		/// The array element type.
		/// Pass null to resolve an implicitly-typed array creation.
		/// </param>
		/// <param name="sizeArguments">
		/// The size arguments.
		/// The length of this array will be used as the number of dimensions of the array type.
		/// Negative values will be treated as errors.
		/// </param>
		/// <param name="initializerElements">
		/// The initializer elements. May be null if no array initializer was specified.
		/// The resolver may mutate this array to wrap elements in <see cref="CastExpression"/>s!
		/// </param>
		public ArrayCreateExpression ResolveArrayCreation(IType elementType, int[] sizeArguments, Expression[] initializerElements = null)
		{
			Expression[] sizeArgResults = new Expression[sizeArguments.Length];
			for (int i = 0; i < sizeArguments.Length; i++) {
				if (sizeArguments[i] < 0)
					sizeArgResults[i] = ErrorExpression.UnknownError;
				else
					sizeArgResults[i] = new ConstantExpression(compilation.FindType(KnownTypeCode.Int32), sizeArguments[i]);
			}
			return ResolveArrayCreation(elementType, sizeArgResults, initializerElements);
		}
		
		/// <summary>
		/// Resolves an array creation.
		/// </summary>
		/// <param name="elementType">
		/// The array element type.
		/// Pass null to resolve an implicitly-typed array creation.
		/// </param>
		/// <param name="sizeArguments">
		/// The size arguments.
		/// The length of this array will be used as the number of dimensions of the array type.
		/// The resolver may mutate this array to wrap elements in <see cref="CastExpression"/>s!
		/// </param>
		/// <param name="initializerElements">
		/// The initializer elements. May be null if no array initializer was specified.
		/// The resolver may mutate this array to wrap elements in <see cref="CastExpression"/>s!
		/// </param>
		public ArrayCreateExpression ResolveArrayCreation(IType elementType, Expression[] sizeArguments, Expression[] initializerElements = null)
		{
			int dimensions = sizeArguments.Length;
			if (dimensions == 0)
				throw new ArgumentException("sizeArguments.Length must not be 0");
			if (elementType == null) {
				TypeInference typeInference = new TypeInference(compilation, conversions);
				bool success;
				elementType = typeInference.GetBestCommonType(initializerElements, out success);
			}
			IType arrayType = new ArrayType(compilation, elementType, dimensions);
			
			AdjustArrayAccessArguments(this,sizeArguments);
			
			if (initializerElements != null) {
				for (int i = 0; i < initializerElements.Length; i++) {
					initializerElements[i] = Convert(initializerElements[i], elementType);
				}
			}
			return new ArrayCreateExpression(arrayType, sizeArguments, initializerElements);
		}
        void AdjustArrayAccessArguments(ResolveContext rc, Expression[] arguments)
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                if (!(rc.TryConvert(ref arguments[i], rc.compilation.FindType(KnownTypeCode.Int32)) ||
                      rc.TryConvert(ref arguments[i], rc.compilation.FindType(KnownTypeCode.UInt32)) ||
                     rc.TryConvert(ref arguments[i], rc.compilation.FindType(KnownTypeCode.Int64)) ||
                      rc.TryConvert(ref arguments[i], rc.compilation.FindType(KnownTypeCode.UInt64))))
                {
                    // conversion failed
                    arguments[i] = rc.Convert(arguments[i], rc.compilation.FindType(KnownTypeCode.Int32), Conversion.None);
                }
            }
        }
		#endregion
		
		
		#region ResolveAssignment
        /// <summary>
        /// Gets the binary operator for the specified compound assignment operator.
        /// Returns null if 'op' is not a compound assignment.
        /// </summary>
        public static BinaryOperatorType? GetCorrespondingBinaryOperator(AssignmentOperatorType op)
        {
            switch (op)
            {
                case AssignmentOperatorType.Assign:
                    return null;
                case AssignmentOperatorType.Add:
                    return BinaryOperatorType.Addition;
                case AssignmentOperatorType.Subtract:
                    return BinaryOperatorType.Subtraction;
                case AssignmentOperatorType.Multiply:
                    return BinaryOperatorType.Multiply;
                case AssignmentOperatorType.Divide:
                    return BinaryOperatorType.Division;
                case AssignmentOperatorType.Modulus:
                    return BinaryOperatorType.Modulus;
                case AssignmentOperatorType.ShiftLeft:
                    return BinaryOperatorType.LeftShift;
                case AssignmentOperatorType.ShiftRight:
                    return BinaryOperatorType.RightShift;
                case AssignmentOperatorType.RotateLeft:
                    return BinaryOperatorType.RotateLeft;
                case AssignmentOperatorType.RotateRight:
                    return BinaryOperatorType.RotateRight;

                case AssignmentOperatorType.BitwiseAnd:
                    return BinaryOperatorType.BitwiseAnd;
                case AssignmentOperatorType.BitwiseOr:
                    return BinaryOperatorType.BitwiseOr;
                case AssignmentOperatorType.ExclusiveOr:
                    return BinaryOperatorType.ExclusiveOr;
                default:
                    throw new NotSupportedException("Invalid value for AssignmentOperatorType");
            }
        }
		public Expression ResolveAssignment(AssignmentOperatorType op, Expression lhs, Expression rhs)
		{
			var linqOp = GetLinqNodeType(op, this.CheckForOverflow);
			var bop = GetCorrespondingBinaryOperator(op);
			if (bop == null) {
				return new OperatorExpression(lhs.Type, linqOp, lhs, this.Convert(rhs, lhs.Type));
			}
			Expression bopResult = new VSC.AST.BinaryExpression(bop.Value, lhs, rhs).DoResolve(this);
			OperatorExpression opResult = bopResult as OperatorExpression;
			if (opResult == null || opResult.Operands.Count != 2)
				return bopResult;
			return new OperatorExpression(lhs.Type, linqOp, opResult.UserDefinedOperatorMethod, opResult.IsLiftedOperator,
			                                 new [] { lhs, opResult.Operands[1] });
		}
		#endregion
	}
}
