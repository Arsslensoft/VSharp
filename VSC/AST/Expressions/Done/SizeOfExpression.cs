using System;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   Implements the sizeof expression
    /// </summary>
    public class SizeOfExpression : Expression
    {
        readonly Expression texpr;
        IType referencedType;
        int? constantValue;
      
        
        public SizeOfExpression(IType int32, IType referencedType, int? constantValue)
        {
            if (referencedType == null)
                throw new ArgumentNullException("referencedType");
            this.referencedType = referencedType;
            this.constantValue = constantValue;
            this.ResolvedType = int32;
            this._resolved = true;
        }
        /// <summary>
        /// The type referenced by the 'sizeof'.
        /// </summary>
        public IType ReferencedType
        {
            get { return referencedType; }
        }

        public override bool IsCompileTimeConstant
        {
            get
            {
                return constantValue != null;
            }
        }

        public override object ConstantValue
        {
            get
            {
                return constantValue;
            }
        }


        public SizeOfExpression(Expression queried_type, Location l)
        {
            this.texpr = queried_type;
            loc = l;
        }
        public Expression TypeExpression
        {
            get
            {
                return texpr;
            }
        }
        public int ResolveSizeOfBuiltin(ResolveContext rc,IType type)
        {

            switch (ReflectionHelper.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                    return 1;
     
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                  return 2;
         
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Single:
                    return  4;
                   
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Double:
                    return 8;
                   
            }
            return 0;
        }
        public override Expression DoResolve(ResolveContext rc)
        {
            referencedType = (texpr as ITypeReference).Resolve(rc);
            if (referencedType == null)
                return null;

            if (referencedType.Kind == TypeKind.Enum)
                referencedType = rc.GetEnumUnderlyingType(referencedType);

            int size_of = ResolveSizeOfBuiltin(rc, referencedType);
            if (size_of > 0)
                return new IntConstant(size_of, loc);

            _resolved = true;
            eclass = ExprClass.Value;
            return this;

        }
        //public override IConstantValue BuilConstantValue(bool isAttributeConstant)
        //{
        //    return new SizeOfConstantValue(TypeExpression as ITypeReference);
        //}
    }
}