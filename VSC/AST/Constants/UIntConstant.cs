using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
namespace VSC.AST
{

    public class UIntConstant : PrimitiveConstantExpression
    {
        internal uint _value;
        public UIntConstant(uint value, VSC.Base.GoldParser.Parser.LineInfo loc)
            : base(VSC.TypeSystem.KnownTypeReference.UInt32, value, loc)
        {
            _value = value;
        }

        public override object GetValue()
        {
            return _value;
        }

      
    }
    
	
	
}