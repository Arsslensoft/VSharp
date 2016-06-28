using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST {

    public class IntegralLiteral : LiteralExpression
    {
 
		
        [Rule("<Integral Constant> ::= OctalLiteral")]
        public IntegralLiteral(OctalTerminal _symbol118)
        {
            try
            {
                string value = _symbol118.Name;
                string suffix = GetSuffix(value);
                if (suffix.Length > 0)
                    value = value.Substring(0, value.IndexOf(suffix));

                bool islong = IsLong(suffix);
                ulong val = Convert.ToUInt64(value.Remove(0, 2), 8);
                if (val < byte.MaxValue && !islong)
                {
                    if (!IsUnsigned(suffix) && val < (ulong)sbyte.MaxValue)
                        ConstantExpr = new SByteConstant((sbyte)val, _symbol118.position);
                    else ConstantExpr = new ByteConstant((byte)val, _symbol118.position);
                }
                else if (val < ushort.MaxValue && !islong)
                {
                    if (!IsUnsigned(suffix) && val < (ulong)short.MaxValue)
                        ConstantExpr = new ShortConstant((short)val, _symbol118.position);
                    else ConstantExpr = new UShortConstant((ushort)val, _symbol118.position);
                }
                else if (val < uint.MaxValue && !islong)
                {
                    if (!IsUnsigned(suffix) && val < (ulong)int.MaxValue)
                        ConstantExpr = new IntConstant((int)val, _symbol118.position);
                    else ConstantExpr = new UIntConstant((uint)val, _symbol118.position);
                }
                else
                {
                    if (!IsUnsigned(suffix) && val < (ulong)long.MaxValue)
                        ConstantExpr = new LongConstant((long)val, _symbol118.position);
                    else ConstantExpr = new ULongConstant((ulong)val, _symbol118.position);
                }
              

            }
            catch (Exception ex)
            {

            }
        }
        [Rule("<Integral Constant> ::= HexLiteral")]
        public IntegralLiteral(HexTerminal _symbol118)
        {
            try
            {
                string value = _symbol118.Name;
                string suffix = GetSuffix(value);
                if (suffix.Length > 0)
                    value = value.Substring(0, value.IndexOf(suffix));

                bool islong = IsLong(suffix);
                ulong val = Convert.ToUInt64(value.Remove(0, 2), 16);
                if (val < byte.MaxValue && !islong)
                {
                    if (!IsUnsigned(suffix) && val < (ulong)sbyte.MaxValue)
                        ConstantExpr = new SByteConstant((sbyte)val, _symbol118.position);
                    else ConstantExpr = new ByteConstant((byte)val, _symbol118.position);
                }
                else if (val < ushort.MaxValue && !islong)
                {
                    if (!IsUnsigned(suffix) && val < (ulong)short.MaxValue)
                        ConstantExpr = new ShortConstant((short)val, _symbol118.position);
                    else ConstantExpr = new UShortConstant((ushort)val, _symbol118.position);
                }
                else if (val < uint.MaxValue && !islong)
                {
                    if (!IsUnsigned(suffix) && val < (ulong)int.MaxValue)
                        ConstantExpr = new IntConstant((int)val, _symbol118.position);
                    else ConstantExpr = new UIntConstant((uint)val, _symbol118.position);
                }
                else
                {
                    if (!IsUnsigned(suffix) && val < (ulong)long.MaxValue)
                        ConstantExpr = new LongConstant((long)val, _symbol118.position);
                    else ConstantExpr = new ULongConstant((ulong)val, _symbol118.position);
                }
              
            }
            catch (Exception ex)
            {
               
            }
        }

        [Rule("<Integral Constant> ::= DecLiteral")]
        public IntegralLiteral(DecimalTerminal _symbol118)
        {
            try
            {
                string value = _symbol118.Name;
                string suffix = GetSuffix(value);
                if (suffix.Length > 0)
                    value = value.Substring(0, value.IndexOf(suffix));

                bool islong = IsLong(suffix);
                ulong val = Convert.ToUInt64(value);
                if (val < byte.MaxValue && !islong)
                {
                    if (!IsUnsigned(suffix) && val < (ulong)sbyte.MaxValue)
                        ConstantExpr = new SByteConstant((sbyte)val, _symbol118.position);
                    else ConstantExpr = new ByteConstant((byte)val, _symbol118.position);
                }
                else if (val < ushort.MaxValue && !islong)
                {
                    if (!IsUnsigned(suffix) && val < (ulong)short.MaxValue)
                        ConstantExpr = new ShortConstant((short)val, _symbol118.position);
                    else ConstantExpr = new UShortConstant((ushort)val, _symbol118.position);
                }
                else if (val < uint.MaxValue && !islong)
                {
                    if (!IsUnsigned(suffix) && val < (ulong)int.MaxValue)
                        ConstantExpr = new IntConstant((int)val, _symbol118.position);
                    else ConstantExpr = new UIntConstant((uint)val, _symbol118.position);
                }
                else
                {
                    if (!IsUnsigned(suffix) && val < (ulong)long.MaxValue)
                        ConstantExpr = new LongConstant((long)val, _symbol118.position);
                    else ConstantExpr = new ULongConstant((ulong)val, _symbol118.position);
                }

            }
            catch (Exception ex)
            {

            }
        }
			
        [Rule("<Integral Constant> ::= BinaryLiteral")]
	     public IntegralLiteral( BinaryTerminal _symbol118)
				{
                    try
                    {
                        string value = _symbol118.Name;
                        string suffix = GetSuffix(value);
                        if (suffix.Length > 0)
                            value = value.Substring(0, value.IndexOf(suffix));

                        bool islong = IsLong(suffix);
                        ulong val = Convert.ToUInt64(value.Remove(0, 2), 2);
                        if (val < byte.MaxValue && !islong)
                        {
                            if (!IsUnsigned(suffix) && val < (ulong)sbyte.MaxValue)
                                ConstantExpr = new SByteConstant((sbyte)val, _symbol118.position);
                            else ConstantExpr = new ByteConstant((byte)val, _symbol118.position);
                        }
                        else if (val < ushort.MaxValue && !islong)
                        {
                            if (!IsUnsigned(suffix) && val < (ulong)short.MaxValue)
                                ConstantExpr = new ShortConstant((short)val, _symbol118.position);
                            else ConstantExpr = new UShortConstant((ushort)val, _symbol118.position);
                        }
                        else if (val < uint.MaxValue && !islong)
                        {
                            if (!IsUnsigned(suffix) && val < (ulong)int.MaxValue)
                                ConstantExpr = new IntConstant((int)val, _symbol118.position);
                            else ConstantExpr = new UIntConstant((uint)val, _symbol118.position);
                        }
                        else
                        {
                            if (!IsUnsigned(suffix) && val < (ulong)long.MaxValue)
                                ConstantExpr = new LongConstant((long)val, _symbol118.position);
                            else ConstantExpr = new ULongConstant((ulong)val, _symbol118.position);
                        }

                    }
                    catch (Exception ex)
                    {

                    }
				}
}
}
