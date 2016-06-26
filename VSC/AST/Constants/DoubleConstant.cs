using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;


namespace VSC.AST
{
	
	public class DoubleConstant : ConstantExpression
    {
        double _value;
        public DoubleConstant(double value, VSC.Base.GoldParser.Parser.LineInfo loc)
            : base(VSC.TypeSystem.KnownTypeReference.Double, value, loc)
        {
            _value = value;
        }

        public override object GetValue()
        {
            return _value;
        }
    }
    
	
	
}