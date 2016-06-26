using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;


namespace VSC.AST
{
	
	public class ByteConstant : ConstantExpression
    {
        internal byte _value;
        public ByteConstant(byte value, VSC.Base.GoldParser.Parser.LineInfo loc)
            : base(VSC.TypeSystem.KnownTypeReference.Byte, value, loc)
        {
            _value = value;
        }

        public override object GetValue()
        {
            return _value;
        }
    }
    
	
	
}