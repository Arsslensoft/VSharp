using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using VSC.Base.GoldParser.Parser;

namespace VSC.AST
{
    public class StringConstant : PrimitiveConstantExpression
    {

        string _value;
        public StringConstant(string value, LineInfo loc)
            : base(VSC.TypeSystem.KnownTypeReference.String, value,loc)
        {
            _value = value;
        }
       

        public override object GetValue()
        {
            return _value;
        }
   
     
    }	
}