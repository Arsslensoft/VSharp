using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace VSC.AST
{
	
	public class IntConstant : ConstantExpression
    {
        internal int _value;
        public IntConstant(int value, VSC.Base.GoldParser.Parser.LineInfo loc)
            : base(VSC.TypeSystem.KnownTypeReference.Int32, value, loc)
        {
            _value = value;
        }

        public override object GetValue()
        {
            return _value;
        }
    }
    
	
	
}