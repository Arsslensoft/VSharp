using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;


namespace VSC.AST
{
	
	public class SByteConstant : ConstantExpression
    {
        internal sbyte _value;
        public SByteConstant(sbyte value, VSC.Base.GoldParser.Parser.LineInfo loc)
            : base(VSC.TypeSystem.KnownTypeReference.SByte, value, loc)
        {
            _value = value;
        }

        public override object GetValue()
        {
            return _value;
        }
    }
    
	
	
}