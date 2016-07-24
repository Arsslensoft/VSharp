using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   This kind of cast is used to encapsulate the child
    ///   whose type is child.Type into an expression that is
    ///   reported to return "return_type".  This is used to encapsulate
    ///   expressions which have compatible types, but need to be dealt
    ///   at higher levels with.
    ///
    ///   For example, a "byte" expression could be encapsulated in one
    ///   of these as an "unsigned int".  The type for the expression
    ///   would be "unsigned int".
    ///
    /// </summary>
    public abstract class TypeCast : Expression
    {
        protected readonly Expression child;

        protected TypeCast(Expression child, IType return_type)
        {
            eclass = child.eclass;
            loc = child.Location;
            ResolvedType = return_type;
            this.child = child;
        }

        public Expression Child
        {
            get { return child; }
        }

    }

    /// <remarks>
    ///   The ExprClass class contains the is used to pass the 
    ///   classification of an expression (value, variable, namespace,
    ///   type, method group, property access, event access, indexer access,
    ///   nothing).
    /// </remarks>
    public enum ExprClass : byte
    {
        Unresolved = 0,

        Value,
        Variable,
        Namespace,
        Type,
        TypeParameter,
        MethodGroup,
        PropertyAccess,
        EventAccess,
        IndexerAccess,
        Nothing,
    }

    /// <summary>
	///   This class exists solely to pass the Type around and to be a dummy
	///   that can be passed to the conversion functions (this is used by
	///   foreach implementation to typecast the object return value from
	///   get_Current into the proper type.  All code has been generated and
	///   we only care about the side effect conversions to be performed
	///
	///   This is also now used as a placeholder where a no-action expression
	///   is needed (the `New' class).
	/// </summary>
	public class EmptyExpression : Expression
    {
        sealed class OutAccessExpression : EmptyExpression
        {
   
            public override Expression DoResolveLeftValue(ResolveContext rc, Expression right_side)
            {
                rc.Report.Error(206, right_side.Location,
                    "A property, indexer or dynamic member access may not be passed as `ref' or `out' parameter");

                return null;
            }
        }

        public static readonly EmptyExpression LValueMemberAccess = new EmptyExpression();
        public static readonly EmptyExpression LValueMemberOutAccess = new EmptyExpression();
        public static readonly EmptyExpression UnaryAddress = new EmptyExpression();
        public static readonly EmptyExpression EventAddition = new EmptyExpression();
        public static readonly EmptyExpression EventSubtraction = new EmptyExpression();
        public static readonly EmptyExpression MissingValue = new EmptyExpression();
        public static readonly Expression Null = new EmptyExpression();
        public static readonly EmptyExpression OutAccess = new OutAccessExpression();

        public EmptyExpression()
        {
            ResolvedType = SpecialTypeSpec.FakeType;
            _resolved = true;
            loc = Location.Null;
        }
        

    }
    /// <summary>
    /// Represents an unknown member.
    /// </summary>
    public class UnknownMemberExpression : Expression
    {
        readonly IType targetType;
        readonly string memberName;
        readonly ReadOnlyCollection<IType> typeArguments;

        public UnknownMemberExpression(IType targetType, string memberName, IEnumerable<IType> typeArguments)
           
        {
            if (targetType == null)
                throw new ArgumentNullException("targetType");
            this.targetType = targetType;
            this.memberName = memberName;
            this.ResolvedType = SpecialTypeSpec.UnknownType;
            this.typeArguments = new ReadOnlyCollection<IType>(typeArguments.ToArray());
        }

        /// <summary>
        /// The type on which the method is being called.
        /// </summary>
        public IType TargetType
        {
            get { return targetType; }
        }

        public string MemberName
        {
            get { return memberName; }
        }

        public ReadOnlyCollection<IType> TypeArguments
        {
            get { return typeArguments; }
        }

      
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0} {1}.{2}]", GetType().Name, targetType, memberName);
        }
    }
    public class ErrorExpression : FullNamedExpression
    {	/// <summary>
        /// Gets an ErrorExpression instance with <c>Type</c> = <c>SpecialType.UnknownType</c>.
        /// </summary>
        public static readonly ErrorExpression UnknownError = new ErrorExpression(SpecialTypeSpec.UnknownType);
        public ErrorExpression(Location loc)
        {
            this.loc = loc;
            type = SpecialTypeSpec.UnknownType;
            ResolvedType = SpecialTypeSpec.UnknownType;
        }
        public ErrorExpression(IType t)
     
        {
            ResolvedType = t;
            _resolved = true;

        }

        public ErrorExpression(IType t, Location l)
            :this(l)
        {
            ResolvedType = t;
            _resolved = true;

        }
        public override bool IsError
        {
            get
            {
                return true;
            }
        }
    }

    /// <summary>
    /// Represents a group of methods.
    /// A method reference used to create a delegate is resolved to a MethodGroupResolveResult.
    /// The MethodGroupResolveResult has no type.
    /// To retrieve the delegate type or the chosen overload, look at the method group conversion.
    /// </summary>
    public class MethodGroupExpression : Expression
    {
        readonly IList<MethodListWithDeclaringType> methodLists;
        readonly IList<IType> typeArguments;
        readonly Expression targetResult;
        readonly string methodName;

        public MethodGroupExpression(Expression targetResult, string methodName, IList<MethodListWithDeclaringType> methods, IList<IType> typeArguments)

        {
            this._resolved = true;
            this.ResolvedType = SpecialTypeSpec.UnknownType;
            
            if (methods == null)
                throw new ArgumentNullException("methods");
            this.targetResult = targetResult;
            this.methodName = methodName;
            this.methodLists = methods;
            this.typeArguments = typeArguments ?? EmptyList<IType>.Instance;
        }

        /// <summary>
        /// Gets the resolve result for the target object.
        /// </summary>
        public Expression TargetResult
        {
            get { return targetResult; }
        }

        /// <summary>
        /// Gets the type of the reference to the target object.
        /// </summary>
        public IType TargetType
        {
            get { return targetResult != null ? targetResult.Type : SpecialTypeSpec.UnknownType; }
        }

        /// <summary>
        /// Gets the name of the methods in this group.
        /// </summary>
        public string MethodName
        {
            get { return methodName; }
        }

        /// <summary>
        /// Gets the methods that were found.
        /// This list does not include extension methods.
        /// </summary>
        public IEnumerable<IMethod> Methods
        {
            get { return methodLists.SelectMany(m => m.Cast<IMethod>()); }
        }

        /// <summary>
        /// Gets the methods that were found, grouped by their declaring type.
        /// This list does not include extension methods.
        /// Base types come first in the list.
        /// </summary>
        public IEnumerable<MethodListWithDeclaringType> MethodsGroupedByDeclaringType
        {
            get { return methodLists; }
        }

        /// <summary>
        /// Gets the type arguments that were explicitly provided.
        /// </summary>
        public IList<IType> TypeArguments
        {
            get { return typeArguments; }
        }

        /// <summary>
        /// List of extension methods, used to avoid re-calculating it in ResolveInvocation() when it was already
        /// calculated by ResolveMemberAccess().
        /// </summary>
        internal List<List<IMethod>> extensionMethods;

        // the resolver is used to fetch extension methods on demand
        internal ResolveContext resolver;

        /// <summary>
        /// Gets all candidate extension methods.
        /// Note: this includes candidates that are not eligible due to an inapplicable
        /// this argument.
        /// The candidates will only be specialized if the type arguments were provided explicitly.
        /// </summary>
        /// <remarks>
        /// The results are stored in nested lists because they are grouped by using scope.
        /// That is, for "using SomeExtensions; namespace X { using MoreExtensions; ... }",
        /// the return value will be
        /// new List {
        ///    new List { all extensions from MoreExtensions },
        ///    new List { all extensions from SomeExtensions }
        /// }
        /// </remarks>
        public IEnumerable<IEnumerable<IMethod>> GetExtensionMethods()
        {
            if (resolver != null)
            {
                Debug.Assert(extensionMethods == null);
                try
                {
                    extensionMethods = resolver.GetExtensionMethods(methodName, typeArguments);
                }
                finally
                {
                    resolver = null;
                }
            }
            return extensionMethods ?? Enumerable.Empty<IEnumerable<IMethod>>();
        }

        /// <summary>
        /// Gets the eligible extension methods.
        /// </summary>
        /// <param name="substituteInferredTypes">
        /// Specifies whether to produce a <see cref="SpecializedMethod"/>
        /// when type arguments could be inferred from <see cref="TargetType"/>.
        /// This setting is only used for inferred types and has no effect if the type parameters are
        /// specified explicitly.
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
        public IEnumerable<IEnumerable<IMethod>> GetEligibleExtensionMethods(bool substituteInferredTypes)
        {
            var result = new List<List<IMethod>>();
            foreach (var methodGroup in GetExtensionMethods())
            {
                var outputGroup = new List<IMethod>();
                foreach (var method in methodGroup)
                {
                    IType[] inferredTypes;
                    if (ResolveContext.IsEligibleExtensionMethod(this.TargetType, method, true, out inferredTypes))
                    {
                        if (substituteInferredTypes && inferredTypes != null)
                        {
                            outputGroup.Add(method.Specialize(new TypeParameterSubstitution(null, inferredTypes)));
                        }
                        else
                        {
                            outputGroup.Add(method);
                        }
                    }
                }
                if (outputGroup.Count > 0)
                    result.Add(outputGroup);
            }
            return result;
        }

        public override string ToString()
        {
            return string.Format("[{0} with {1} method(s)]", GetType().Name, this.Methods.Count());
        }

        public OverloadResolution PerformOverloadResolution(ICompilation compilation, Expression[] arguments, string[] argumentNames = null,
                                                            bool allowExtensionMethods = true,
                                                            bool allowExpandingParams = true,
                                                            bool allowOptionalParameters = true,
                                                            bool checkForOverflow = false, VSharpConversions conversions = null)
        {

            var typeArgumentArray = this.TypeArguments.ToArray();
            OverloadResolution or = new OverloadResolution(compilation, arguments, argumentNames, typeArgumentArray, conversions);
            or.AllowExpandingParams = allowExpandingParams;
            or.AllowOptionalParameters = allowOptionalParameters;
            or.CheckForOverflow = checkForOverflow;

            or.AddMethodLists(methodLists);

            if (allowExtensionMethods && !or.FoundApplicableCandidate)
            {
                // No applicable match found, so let's try extension methods.

                var extensionMethods = this.GetExtensionMethods();

                if (extensionMethods.Any())
                {

                    Expression[] extArguments = new Expression[arguments.Length + 1];
                    extArguments[0] = new AST.Expression(this.TargetType);
                    arguments.CopyTo(extArguments, 1);
                    string[] extArgumentNames = null;
                    if (argumentNames != null)
                    {
                        extArgumentNames = new string[argumentNames.Length + 1];
                        argumentNames.CopyTo(extArgumentNames, 1);
                    }
                    var extOr = new OverloadResolution(compilation, extArguments, extArgumentNames, typeArgumentArray, conversions);
                    extOr.AllowExpandingParams = allowExpandingParams;
                    extOr.AllowOptionalParameters = allowOptionalParameters;
                    extOr.IsExtensionMethodInvocation = true;
                    extOr.CheckForOverflow = checkForOverflow;

                    foreach (var g in extensionMethods)
                    {
                        foreach (var method in g)
                            extOr.AddCandidate(method);

                        if (extOr.FoundApplicableCandidate)
                            break;
                    }
                    // For the lack of a better comparison function (the one within OverloadResolution
                    // cannot be used as it depends on the argument set):
                    if (extOr.FoundApplicableCandidate || or.BestCandidate == null)
                    {
                        // Consider an extension method result better than the normal result only
                        // if it's applicable; or if there is no normal result.
                        or = extOr;
                    }
                }
            }

            return or;
        }

       
    }
     /// <remarks>
    ///   Base class for expressions
    /// </remarks>
    public  class Expression : IAstNode, IResolveExpression
     {
        public NameLookupMode lookupMode;
        protected ITypeReference type;
         protected IType ResolvedType;
        protected Location loc;
        public ExprClass eclass;

         public Expression(IType t)
         {
             ResolvedType = t;
             _resolved = true;
         }

         public Expression()
         {
             
         }
         protected bool _resolved = false;
         public bool Resolved
         {
             get { return _resolved; }
         }
        public ITypeReference UnresolvedTypeReference
        {
            get { return type; }
            set { type = value; }
        }
        public IType Type
        {
            get { return ResolvedType; }
            set { ResolvedType = value; }
        }
        public IAstNode ParentNode { get; set; }

        public Location Location
        {
            get { return loc; }
        }

        public virtual bool IsNull
        {
            get
            {
                return false;
            }
        }
        public virtual bool IsCompileTimeConstant
        {
            get { return false; }
        }

        public virtual object ConstantValue
        {
            get { return null; }
        }

        public virtual bool IsError
        {
            get { return false; }
        }

        public static ErrorExpression ErrorResult = new ErrorExpression(Location.Null);
        public virtual string GetSignatureForError()
        {
            return type.ToString();
        }
        public virtual void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }

        public virtual Expression DoResolve(ResolveContext rc)
        {
            return this;
        }
        public virtual Expression DoResolveLeftValue(ResolveContext rc, Expression right)
        {
            return this;
        }

         public IType ResolveAsType(ResolveContext rc)
         {
             this.lookupMode = NameLookupMode.Type;
             return (this as ITypeReference).Resolve(rc);
         }
         public Expression ResolveAsInvocationTarget(ResolveContext rc)
         {
             this.lookupMode = NameLookupMode.InvocationTarget;
             return DoResolve(rc);
         }
         public Expression ResolveAsTypeInUsingDeclaration(ResolveContext rc)
         {
             this.lookupMode = NameLookupMode.TypeInUsingDeclaration;
             return DoResolve(rc);
         }
         public Expression ResolveAsBaseTypeReference(ResolveContext rc)
         {
             this.lookupMode = NameLookupMode.BaseTypeReference;
             return DoResolve(rc);
         }

        #region Constant Folding
        // Constant folding
        public virtual AST.Expression Constantify(ResolveContext resolver)
        {
            return this;
        }
        public AST.Expression ResolveConstant(ITypeResolveContext context)
        {
            var csContext = (VSharpTypeResolveContext)context;
            if (context.CurrentAssembly != context.Compilation.MainAssembly)
            {
                // The constant needs to be resolved in a different compilation.
                IProjectContent pc = context.CurrentAssembly as IProjectContent;
                if (pc != null)
                {
                    ICompilation nestedCompilation = context.Compilation.SolutionSnapshot.GetCompilation(pc);
                    if (nestedCompilation != null)
                    {
                        var nestedContext = MapToNestedCompilation(csContext, nestedCompilation);
                        AST.Expression rr = Constantify(new ResolveContext(nestedContext, CompilerContext.report));
                        return MapToNewContext(rr, context);
                    }
                }
            }
            // Resolve in current context.
            return Constantify(new ResolveContext(csContext, CompilerContext.report));
        }
        VSharpTypeResolveContext MapToNestedCompilation(VSharpTypeResolveContext context, ICompilation nestedCompilation)
        {
            var nestedContext = new VSharpTypeResolveContext(nestedCompilation.MainAssembly);
            if (context.CurrentUsingScope != null)
            {
                nestedContext = nestedContext.WithUsingScope(context.CurrentUsingScope.UnresolvedUsingScope.ResolveScope(nestedCompilation));
            }
            if (context.CurrentTypeDefinition != null)
            {
                nestedContext = nestedContext.WithCurrentTypeDefinition(nestedCompilation.Import(context.CurrentTypeDefinition));
            }
            return nestedContext;
        }
        static AST.Expression MapToNewContext(AST.Expression rr, ITypeResolveContext newContext)
        {
            if (rr is TypeOfExpression)
            {
                return new TypeOfExpression(
                    rr.Type.ToTypeReference().Resolve(newContext),
                    ((TypeOfExpression)rr).TargetType.ToTypeReference().Resolve(newContext));
            }
            else if (rr is ArrayCreation)
            {
                ArrayCreation acrr = (ArrayCreation)rr;
                return new ArrayCreation(
                    acrr.Type.ToTypeReference().Resolve(newContext),
                    MapToNewContext(acrr.arguments, newContext),
                    MapToNewContext(acrr.initializers != null ? acrr.initializers.Elements : null, newContext));
            }
            else if (rr.IsCompileTimeConstant)
            {
                return Constant.CreateConstantFromValue(newContext.Compilation, rr.Type.ToTypeReference().Resolve(newContext),
                    rr.ConstantValue, rr.Location);

            }
            else
            {
                return new ErrorExpression(rr.Type.ToTypeReference().Resolve(newContext));
            }
        }
        static AST.Expression[] MapToNewContext(IList<AST.Expression> input, ITypeResolveContext newContext)
        {
            if (input == null)
                return null;
            AST.Expression[] output = new AST.Expression[input.Count];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = MapToNewContext(input[i], newContext);
            }
            return output;
        }
        #endregion



        public virtual bool HasConditionalAccess()
         {
             return false;
         }
         protected static bool IsNullPropagatingValid(IType type)
         {
             switch (type.Kind)
             {
                 case TypeKind.Struct:
                     return NullableType.IsNullable(type);
                 case TypeKind.Enum:
                 case TypeKind.Void:
                 case TypeKind.Pointer:
                     return false;
                 case TypeKind.Dynamic:
                     return true;
                 case TypeKind.TypeParameter:
                     return !((TypeParameterSpec)type).HasValueTypeConstraint;
                 default:
                     return true;
             }
         }
         public virtual string ExprClassName
         {
             get
             {
                 switch (eclass)
                 {
                     case ExprClass.Unresolved:
                         return "Unresolved";
                     case ExprClass.Value:
                         return "value";
                     case ExprClass.Variable:
                         return "variable";
                     case ExprClass.Namespace:
                         return "package";
                     case ExprClass.Type:
                         return "type";
                     case ExprClass.MethodGroup:
                         return "method group";
                     case ExprClass.PropertyAccess:
                         return "property access";
                     case ExprClass.EventAccess:
                         return "event access";
                     case ExprClass.IndexerAccess:
                         return "indexer access";
                     case ExprClass.Nothing:
                         return "null";
                     case ExprClass.TypeParameter:
                         return "type parameter";
                 }
                 throw new Exception("Should not happen");
             }
         }



         public virtual Expression ShallowClone()
         {
             return (Expression)MemberwiseClone();
         }
    }
}
