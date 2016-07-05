using VSC.TypeSystem;

namespace VSC.AST
{
    /// <summary>
    ///   Expression that evaluates to a type
    /// </summary>
    public abstract class TypeExpr : FullNamedExpression, ITypeReference
    {
        public abstract   IType Resolve(ITypeResolveContext context);
      
        public override bool Equals(object obj)
        {
            TypeExpr tobj = obj as TypeExpr;
            if (tobj == null)
                return false;

            return Type == tobj.Type;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }
    }
}