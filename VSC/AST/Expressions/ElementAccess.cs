using System;
using System.Collections.Generic;
using System.Linq;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   An Element Access expression.
    ///
    ///   During semantic analysis these are transformed into 
    ///   IndexerAccess, ArrayAccess or a PointerArithmetic.
    /// </summary>
    public class ElementAccess : Expression
    {
        public Arguments Arguments;
        public Expression Expr;
        bool conditional_access_receiver;

        public ElementAccess(Expression e, Arguments args, Location loc)
        {
            Expr = e;
            this.loc = loc;
            this.Arguments = args;
        }

        public bool ConditionalAccess { get; set; }
        public override bool HasConditionalAccess()
        {
            return ConditionalAccess || Expr.HasConditionalAccess();
        }
        //
        // We perform some simple tests, and then to "split" the emit and store
        // code we create an instance of a different class, and return that.
        //
        Expression ResolveAccessExpression(ResolveContext rc, bool conditionalAccessReceiver)
        {
            Expr = Expr.DoResolve(rc);
            if (Expr == null)
                return null;

            ResolvedType = Expr.Type;

            if (ConditionalAccess && !IsNullPropagatingValid(ResolvedType))
            {
                rc.Report.Error(0, loc, "The `{0}' operator cannot be applied to operand of type `{1}'",
                        "?", ResolvedType.ToString());
                return null;
            }
            	string[] argumentNames;
                Expression[] arguments = Arguments.GetArguments(out argumentNames);
           
            return ResolveIndexer(rc, Expr, arguments, argumentNames);
        }
        public override Expression DoResolve(ResolveContext rc)
        {

            var expr = ResolveAccessExpression(rc, false);
            if (expr == null)
                return null;

            return expr.DoResolve(rc);
        }
        public override Expression DoResolveLeftValue(ResolveContext rc, Expression right)
        {
            var res = ResolveAccessExpression(rc, false);
            if (res == null)
                return null;

            return res.DoResolveLeftValue(rc, right);
        }


        #region ResolveIndexer
        /// <summary>
        /// Resolves an indexer access.
        /// </summary>
        /// <param name="target">Target expression.</param>
        /// <param name="arguments">
        /// Arguments passed to the indexer.
        /// The resolver may mutate this array to wrap elements in <see cref="CastExpression"/>s!
        /// </param>
        /// <param name="argumentNames">
        /// The argument names. Pass the null string for positional arguments.
        /// </param>
        /// <returns>ArrayAccessResolveResult, InvocationResolveResult, or ErrorResolveResult</returns>
        public Expression ResolveIndexer(ResolveContext rc,Expression target, Expression[] arguments, string[] argumentNames = null)
        {
            switch (target.Type.Kind)
            {
                case TypeKind.Array:
                    // §7.6.6.1 Array access
                    if(argumentNames != null && argumentNames.Length > 0)
                        rc.Report.Error(0, Arguments.args.Where(x => x is NamedArgument).First().loc, "An element access expression cannot use named argument");


                    return new ArrayAccessExpression(((ElementTypeSpec)target.Type).ElementType, target, arguments);
                case TypeKind.Pointer:
                    // §18.5.3 Pointer element access
                    if (argumentNames != null && argumentNames.Length > 0)
                        rc.Report.Error(0, Arguments.args.Where(x => x is NamedArgument).First().loc, "An element access expression cannot use named argument");

                    return new PointerArithmeticExpression(((ElementTypeSpec)target.Type).ElementType, target, arguments);
            }

            // §7.6.6.2 Indexer access
            MemberLookup lookup = rc.CreateMemberLookup();
            var indexers = lookup.LookupIndexers(target);

            OverloadResolution or = rc.CreateOverloadResolution(arguments, argumentNames);
            or.AddMethodLists(indexers);
            if (or.BestCandidate != null)
                return or.CreateInvocation(target);
            else
            {
                rc.Report.Error(0, loc, "Cannot apply indexing with [] to an expression of type `{0}'",
                    target.Type.ToString());
                return null;
            }
         
        }


        #endregion
    }

    /// <summary>
    /// ResolveScope result representing an array access.
    /// </summary>
    public class ArrayAccessExpression : Expression
    {
        public readonly Expression Array;
        public readonly Expression[] Indexes;
        public bool ConditionalAccess { get; set; }
        public bool ConditionalAccessReceiver { get; set; }
        public ArrayAccessExpression(IType elementType, Expression array, Expression[] indexes)
            : base(elementType)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (indexes == null)
                throw new ArgumentNullException("indexes");
            this.Array = array;
            this.Indexes = indexes;
        }
        /// <summary>
        /// Converts all arguments to int,uint,long or ulong.
        /// </summary>
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

        public override Expression DoResolve(ResolveContext rc)
        {
            AdjustArrayAccessArguments(rc, Indexes);
            var ac = Array.Type as ArrayType;
            if (ac.Dimensions != Indexes.Length)
            {
                rc.Report.Error(0, Array.Location, "Wrong number of indexes `{0}' inside [], expected `{1}'",
                      Indexes.Length.ToString(), ac.Dimensions.ToString());
                return null;
            }
            //convert indexes
            AdjustArrayAccessArguments(rc, Indexes);

            eclass = ExprClass.Value;
            ResolvedType = ac.ElementType;
            _resolved = true;
            return this;
        }
        public override Expression DoResolveLeftValue(ResolveContext rc, Expression right)
        {
            if (ConditionalAccess)
                throw new NotSupportedException("null propagating operator assignment");

            return DoResolve(rc);
        }
    }

    /// <summary>
    /// ResolveScope result representing an array access.
    /// </summary>
    public class PointerArithmeticExpression : Expression
    {
        public readonly Expression Array;
        public readonly Expression[] Indexes;

        public PointerArithmeticExpression(IType elementType, Expression array, Expression[] indexes)
            : base(elementType)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (indexes == null)
                throw new ArgumentNullException("indexes");
            this.Array = array;
            this.Indexes = indexes;
        }
        public override Expression DoResolve(ResolveContext rc)
        {
            AdjustArrayAccessArguments(rc, Indexes);
            var ac = Array.Type as PointerTypeSpec;
            if (1 != Indexes.Length)
            {
                rc.Report.Error(0, Array.Location, "Wrong number of indexes `{0}' inside [], expected `{1}'",
                      Indexes.Length.ToString(),"1");
                return null;
            }
            //convert indexes
            AdjustArrayAccessArguments(rc, Indexes);

            eclass = ExprClass.Value;
            ResolvedType = ac.ElementType;
            _resolved = true;
            return this;
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
    }


}

