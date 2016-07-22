using System.Collections.Generic;
using VSC.Base;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class NewExpression : ExpressionStatement
    {
        protected Arguments arguments;

        //
        // During bootstrap, it contains the RequestedType,
        // but if `type' is not null, it *might* contain a NewDelegate
        // (because of field multi-initialization)
        //
        protected Expression RequestedType;


        public NewExpression(Expression requested_type, Arguments arguments, Location l)
        {
            RequestedType = requested_type;
            this.arguments = arguments;
            loc = l;
        }

        #region Properties
        public Arguments Arguments
        {
            get
            {
                return arguments;
            }
        }
        public Expression TypeExpression
        {
            get
            {
                return RequestedType;
            }
        }

        #endregion


        //TODO:Perform conversion + Assign+Array Init tests
        void HandleElementInitializer(ResolveContext rc, ElementInitializer namedExpression, List<Expression> initializerStatements)
        {
     
            Expression rhs = namedExpression.Source;
            Expression lhsRR = rc.ResolveIdentifierInObjectInitializer(namedExpression.Name);
            if (rhs is ArrayInitializer)
                    initializerStatements.AddRange(ResolveObjectInitializer(rc,lhsRR, (ArrayInitializer)rhs));
            else
            {
                var rhsRR = rhs.DoResolve(rc);
                var rr = rc.ResolveAssignment(AssignmentOperatorType.Assign, lhsRR, rhsRR) as Assign;
                if (rr != null)
                    initializerStatements.Add(rr);
                
            }
        }
        List<Expression> ResolveObjectInitializer(ResolveContext rc, Expression initializedObject, ArrayInitializer initializer)
        {
            //if (this is NewInitializeExpression)
            //{
            //    NewInitializeExpression ni = this as NewInitializeExpression;
            //    List<Expression> initializerStatements = new List<Expression>();
            //    rc = rc.PushObjectInitializer(initializedObject);
            //    foreach (Expression element in initializer.Elements)
            //    {
            //        ArrayInitializer aie = element as ArrayInitializer;
            //        if (aie != null)
            //        {
            //            // constructor argument list in collection initializer
            //            Expression[] addArguments = new Expression[aie.Elements.Count];
            //            int i = 0;
            //            foreach (var addArgument in aie.Elements)
            //                addArguments[i++] = addArgument.DoResolve(rc);
                        
            //            MemberLookup memberLookup = rc.CreateMemberLookup();
            //            var addRR = memberLookup.Lookup(initializedObject, "Add", EmptyList<IType>.Instance, true);
            //            var mgrr = addRR as MethodGroupExpression;
            //            if (mgrr != null)
            //            {
            //                OverloadResolution or = mgrr.PerformOverloadResolution(rc.Compilation, addArguments, null, false, false, false, resolver.CheckForOverflow, resolver.conversions);
            //                var invocationRR = or.CreateInvocation(initializedObject);
            //                initializerStatements.Add(invocationRR);
            //            }
            //        }
            //        else if (element is ElementInitializer)
            //            HandleElementInitializer((ElementInitializer)element, initializerStatements);
                    
            //    }
            //    rc = rc.PopObjectInitializer();
            
            //}
            return null;
        }
       
        public override Expression DoResolve(ResolveContext rc)
        {
            ResolvedType = RequestedType.ResolveAsType(rc);
            if (ResolvedType == null)
                return null;

            eclass = ExprClass.Value;

            //if (ResolvedType.Kind == TypeKind.Delegate)
            //{
            //    return (new NewDelegate(type, arguments, loc)).Resolve(rc);
            //}

            var tparam = ResolvedType as TypeParameterSpec;
            if (tparam != null)
            {
                //
                // Check whether the type of type parameter can be constructed. BaseType can be a struct for method overrides
                // where type parameter constraint is inflated to struct
                //
                if (tparam.HasDefaultConstructorConstraint && !tparam.HasValueTypeConstraint)
                {
                    rc.Report.Error(304, loc,
                        "Cannot create an instance of the variable type `{0}' because it does not have the new() constraint",
                        ResolvedType.ToString());
                }

                if ((arguments != null) && (arguments.Count != 0))
                {
                    rc.Report.Error(417, loc,
                        "`{0}': cannot provide arguments when creating an instance of a variable type",
                        ResolvedType.ToString());
                }

                return this;
            }

            if ((ResolvedType as ITypeDefinition).IsStatic)
            {
                rc.Report.Error(712, loc, "Cannot create an instance of the static class `{0}'", ResolvedType.ToString());
                return null;
            }

            if (ResolvedType.Kind == TypeKind.Interface || (ResolvedType as ITypeDefinition).IsAbstract)
            {
                rc.Report.Error(144, loc, "Cannot create an instance of the abstract class or interface `{0}'", ResolvedType.ToString());
                return null;
            }

            //args
            bool dynamic;
            if (arguments != null)
                arguments.Resolve(rc, out dynamic);
            else
                dynamic = false;


           // //Resolve Object Initializer
           // List<Expression> initializerStatements = ResolveObjectInitializer(rc, new InitializedObjectExpression(ResolvedType), initializer);


           // string[] argumentNames;
           //Expression[]  rarguments = Arguments.GetArguments(out argumentNames);

           //Expression rr = rc.ResolveObjectCreation(ResolvedType, rarguments, argumentNames, false, initializerStatements);
          
            // constructor lookup
            //method = ConstructorLookup(ec, type, ref arguments, loc);

            // TODO:Support dynamics
            //if (dynamic)
            //{
            //    arguments.Insert(0, new Argument(new TypeOf(type, loc).Resolve(ec), Argument.AType.DynamicTypeName));
            //    return new DynamicConstructorBinder(type, arguments, loc).Resolve(ec);
            //}

            return this;
        }

    
        //public override IConstantValue BuilConstantValue( bool isAttributeConstant)
        //{
        //    if (Arguments != null && Arguments.Count == 0)
        //    {
        //        // built in primitive type constants can be created with new
        //        // Todo: correctly resolve the type instead of doing the string approach
        //        switch (RequestedType.ToString())
        //        {
        //            case "Std.Boolean":
        //            case "bool":
        //                return new PrimitiveConstantExpression(KnownTypeReference.Boolean, new bool());
        //            case "Std.Char":
        //            case "char":
        //                return new PrimitiveConstantExpression(KnownTypeReference.Char, new char());
        //            case "Std.SByte":
        //            case "sbyte":
        //                return new PrimitiveConstantExpression(KnownTypeReference.SByte, new sbyte());
        //            case "Std.Byte":
        //            case "byte":
        //                return new PrimitiveConstantExpression(KnownTypeReference.Byte, new byte());
        //            case "Std.Int16":
        //            case "short":
        //                return new PrimitiveConstantExpression(KnownTypeReference.Int16, new short());
        //            case "Std.UInt16":
        //            case "ushort":
        //                return new PrimitiveConstantExpression(KnownTypeReference.UInt16, new ushort());
        //            case "Std.Int32":
        //            case "int":
        //                return new PrimitiveConstantExpression(KnownTypeReference.Int32, new int());
        //            case "Std.UInt32":
        //            case "uint":
        //                return new PrimitiveConstantExpression(KnownTypeReference.UInt32, new uint());
        //            case "Std.Int64":
        //            case "long":
        //                return new PrimitiveConstantExpression(KnownTypeReference.Int64, new long());
        //            case "Std.UInt64":
        //            case "ulong":
        //                return new PrimitiveConstantExpression(KnownTypeReference.UInt64, new ulong());
        //            case "Std.Single":
        //            case "float":
        //                return new PrimitiveConstantExpression(KnownTypeReference.Single, new float());
        //            case "Std.Double":
        //            case "double":
        //                return new PrimitiveConstantExpression(KnownTypeReference.Double, new double());
        //            case "Std.Decimal":
        //            case "decimal":
        //                return new PrimitiveConstantExpression(KnownTypeReference.Decimal, new decimal());
        //        }
        //    }

        //    return null;
        //}
    }
}