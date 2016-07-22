using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   Represents the `self' construct
    /// </summary>
    public class SelfReference : VariableReference
    {
        public SelfReference(IType t,Location loc)
        {
            this.loc = loc;
            _resolved = true;
            ResolvedType = t;
        }

        public SelfReference(Location loc)
        {
            this.loc = loc;
        }

        public override string Name
        {
            get { return "self"; }
        }
         protected  bool causesNonVirtualInvocation = false;
        /// <summary>
        /// Gets whether this resolve result causes member invocations to be non-virtual.
        /// </summary>
        public bool CausesNonVirtualInvocation
        {
            get { return causesNonVirtualInvocation; }
        }
        public static bool IsSelfOrSuperAvailable(ResolveContext rc, bool ignoreAnonymous)
        {
            if (rc.IsStatic || rc.HasAny(ResolveContext.Options.FieldInitializerScope | ResolveContext.Options.BaseInitializer | ResolveContext.Options.ConstantScope))
                return false;

            if (ignoreAnonymous || rc.CurrentAnonymousMethod == null)
                return true;

            if (rc.CurrentTypeDefinition.Kind == TypeKind.Struct )/*&& !(rc.CurrentAnonymousMethod is StateMachineInitializer))*/ //TODO:Anonymous+Iterators
                return false;

            return true;
        }

        public void ResolveSelfType(ResolveContext rc)
        {
            eclass = ExprClass.Variable;
            if (!IsSelfOrSuperAvailable(rc, false))
            {
                if (rc.IsStatic && !rc.HasSet(ResolveContext.Options.ConstantScope))
                    rc.Report.Error(241, loc, "Keyword `self' is not valid in a static property, static method, or static field initializer");
                else if (rc.CurrentAnonymousMethod != null)
                    rc.Report.Error(242, loc,
                        "Anonymous methods inside structs cannot access instance members of `self'. " +
                        "Consider copying `self' to a local variable outside the anonymous method and using the local instead");
                else
                    rc.Report.Error(243, loc, "Keyword `self' is not available in the current context");

                return;
            }


            ITypeDefinition t = rc.CurrentTypeDefinition;
            if (t != null)
            {
                if (t.TypeParameterCount != 0)
                    // Self-parameterize the type
                    ResolvedType = new ParameterizedTypeSpec(t, t.TypeParameters);
                else
                    ResolvedType = t;
            
            }

            
            _resolved = true;
       
        }
        public override Expression DoResolve(ResolveContext rc)
        {
            if (_resolved)
                return this;

            ResolveSelfType(rc);

            //TODO: Anonymous+Iterators
            /* var block = ec.CurrentBlock;
             if (block != null)
             {
                 var top = block.ParametersBlock.TopBlock;
                 if (top.ThisVariable != null)
                     variable_info = top.ThisVariable.VariableInfo;

                 AnonymousExpression am = ec.CurrentAnonymousMethod;
                 if (am != null && ec.IsVariableCapturingRequired && !block.Explicit.HasCapturedThis)
                 {
                     //
                     // Hoisted this is almost like hoisted variable but not exactly. When
                     // there is no variable hoisted we can simply emit an instance method
                     // without lifting this into a storey. Unfotunatelly this complicates
                     // things in other cases because we don't know where this will be hoisted
                     // until top-level block is fully resolved
                     //
                     top.AddThisReferenceFromChildrenBlock(block.Explicit);
                     am.SetHasThisAccess();
                 }
             }*/
            return this;
           
        }
        public override Expression DoResolveLeftValue(ResolveContext rc, Expression right)
        {
            if (_resolved)
                return this;

            ResolveSelfType(rc);

            if (ResolvedType.IsReferenceType.HasValue && ResolvedType.IsReferenceType.Value)
            {
                if (right == EmptyExpression.UnaryAddress)
                    rc.Report.Error(238, loc, "Cannot take the address of `self' because it is read-only");
                else if (right == EmptyExpression.OutAccess)
                    rc.Report.Error(239, loc, "Cannot pass `self' as a ref or out argument because it is read-only");
                else
                    rc.Report.Error(240, loc, "Cannot assign to `self' because it is read-only");
            }



            return this;
        }
    }
}