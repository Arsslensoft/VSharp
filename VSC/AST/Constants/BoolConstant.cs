using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using VSC.Base.GoldParser.Parser;
using VSC.TypeSystem;

namespace VSC.AST
{
    public class BoolConstant : PrimitiveConstantExpression
    {
        internal bool _value;
        public BoolConstant(bool value, LineInfo loc)
            : base(KnownTypeReference.Boolean,value, loc)
        {
            _value = value;
        }
    
        public override object GetValue()
        {
            return _value;
        }
       
    }
    
	
	
	
}