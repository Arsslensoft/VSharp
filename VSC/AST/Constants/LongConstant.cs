using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace VSC.AST
{
	 public class LongConstant : ConstantExpression
    {
         long _value;
        public LongConstant(long value, VSC.Base.GoldParser.Parser.LineInfo loc)
            : base(VSC.TypeSystem.KnownTypeReference.Int64, value, loc)
        {
            _value = value;
        }

        public override object GetValue()
        {
            return _value;
        }
      
    }
   
	
	
	
}