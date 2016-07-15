using VSC.TypeSystem;
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

        public override IConstantValue BuilConstantValue( bool isAttributeConstant)
        {
            if (Arguments != null && Arguments.Count == 0)
            {
                // built in primitive type constants can be created with new
                // Todo: correctly resolve the type instead of doing the string approach
                switch (RequestedType.ToString())
                {
                    case "Std.Boolean":
                    case "bool":
                        return new PrimitiveConstantExpression(KnownTypeReference.Boolean, new bool());
                    case "Std.Char":
                    case "char":
                        return new PrimitiveConstantExpression(KnownTypeReference.Char, new char());
                    case "Std.SByte":
                    case "sbyte":
                        return new PrimitiveConstantExpression(KnownTypeReference.SByte, new sbyte());
                    case "Std.Byte":
                    case "byte":
                        return new PrimitiveConstantExpression(KnownTypeReference.Byte, new byte());
                    case "Std.Int16":
                    case "short":
                        return new PrimitiveConstantExpression(KnownTypeReference.Int16, new short());
                    case "Std.UInt16":
                    case "ushort":
                        return new PrimitiveConstantExpression(KnownTypeReference.UInt16, new ushort());
                    case "Std.Int32":
                    case "int":
                        return new PrimitiveConstantExpression(KnownTypeReference.Int32, new int());
                    case "Std.UInt32":
                    case "uint":
                        return new PrimitiveConstantExpression(KnownTypeReference.UInt32, new uint());
                    case "Std.Int64":
                    case "long":
                        return new PrimitiveConstantExpression(KnownTypeReference.Int64, new long());
                    case "Std.UInt64":
                    case "ulong":
                        return new PrimitiveConstantExpression(KnownTypeReference.UInt64, new ulong());
                    case "Std.Single":
                    case "float":
                        return new PrimitiveConstantExpression(KnownTypeReference.Single, new float());
                    case "Std.Double":
                    case "double":
                        return new PrimitiveConstantExpression(KnownTypeReference.Double, new double());
                    case "Std.Decimal":
                    case "decimal":
                        return new PrimitiveConstantExpression(KnownTypeReference.Decimal, new decimal());
                }
            }

            return null;
        }
    }
}