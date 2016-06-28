using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;


namespace VSC.AST
{

    public class CharConstant : PrimitiveConstantExpression
    {
        internal char _value;
        public CharConstant(char value, VSC.Base.GoldParser.Parser.LineInfo loc)
            : base(VSC.TypeSystem.KnownTypeReference.Char, value, loc)
        {
            _value = value;
        }

        public override object GetValue()
        {
            return _value;
        }
    }
    
	
	
}