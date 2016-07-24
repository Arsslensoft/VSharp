using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   Implements the typeof operator
    /// </summary>
    public class TypeOfExpression : Expression, IConstantValue
    {
        FullNamedExpression QueriedType;
      public  IType TargetType = null;
        public TypeOfExpression(FullNamedExpression queried_type, Location l)
        {
            QueriedType = queried_type;
            loc = l;
        }

        public TypeOfExpression(IType systemType, IType referencedType)
        {
            ResolvedType = systemType;
            TargetType = referencedType;
            _resolved = true;
        }

        #region Properties

        public FullNamedExpression TypeExpression
        {
            get
            {
                return QueriedType;
            }
        }

        #endregion
        public override Expression DoResolve(ResolveContext rc)
        {
            if (_resolved)
                return this;

        
                //
                // Pointer types are allowed without explicit unsafe, they are just tokens
                //
                using (rc.Set(ResolveContext.Options.UnsafeScope))
                TargetType = (QueriedType as ITypeReference).Resolve(rc);
                

                if (TargetType == null)
                    return null;


            ResolvedType = KnownTypeReference.Type.Resolve(rc);

            /*  if (ResolvedType.BuiltinType == BuiltinTypeSpec.Type.Dynamic)
              {
                  rc.Report.Error(1962, QueriedType.Location,
                      "The typeof operator cannot be used on the dynamic type");
                   }
          */ // TODO:Dynamic support
            eclass = ExprClass.Value;
            return this;

        }
        public override Expression Constantify(ResolveContext resolver)
        {
            return DoResolve(resolver);
        }
    }
}