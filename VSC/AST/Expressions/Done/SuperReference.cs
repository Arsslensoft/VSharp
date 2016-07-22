using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class SuperReference : SelfReference
    {
        public SuperReference(Location loc)
            : base(loc)
        {
        }
      
        public void ResolveSuperType(ResolveContext rc)
        {
            eclass = ExprClass.Variable;
            if (!IsSelfOrSuperAvailable(rc, false))
            {
                if (rc.IsStatic)
                    rc.Report.Error(244, loc, "Keyword `super' is not available in a static method");
                else
                    rc.Report.Error(245, loc, "Keyword `super' is not available in the current context");
                
                return;
            }


            ITypeDefinition t = rc.CurrentTypeDefinition;
            if (t != null)
            {
                foreach (IType baseType in t.DirectBaseTypes)
                {
                    if (baseType.Kind != TypeKind.Unknown && baseType.Kind != TypeKind.Interface)
                    {
                        causesNonVirtualInvocation = true;
                        ResolvedType = baseType;
                    }
                }
            }

        }
        public override Expression DoResolve(ResolveContext rc)
        {
            if (_resolved)
                return this;

            ResolveSuperType(rc);

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

            ResolveSuperType(rc);

            return this;
        }
        public override string Name
        {
            get
            {
                return "super";
            }
        }

    }
}