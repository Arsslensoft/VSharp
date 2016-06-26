using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;


namespace VSC.AST
{
	
	public class NullConstant : ConstantExpression
    {
        int _value;
        public NullConstant(VSC.Base.GoldParser.Parser.LineInfo loc)
            : base(VSC.TypeSystem.KnownTypeReference.Object, 0, loc)
        {
            _value = 0;
        }

        public override object GetValue()
        {
            return "null";
        }
    }
	
	
}