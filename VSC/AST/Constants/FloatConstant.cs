using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace VSC.AST
{
	
	public class FloatConstant : ConstantExpression
    {
        internal float _value;
        public FloatConstant(float value, VSC.Base.GoldParser.Parser.LineInfo loc)
            : base(VSC.TypeSystem.KnownTypeReference.Single, value, loc)
        {
            _value = value;
        }

        public override object GetValue()
        {
            return _value;
        }
    }
}