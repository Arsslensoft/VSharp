using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using VSC.Base.GoldParser.Parser;

namespace VSC.AST
{
	 public class StringConstant : ConstantExpression
    {

        string _value;
        public bool Verbatim { get; set; }
        public StringConstant(string value, LineInfo loc,bool verb = false)
            : base(VSC.TypeSystem.KnownTypeReference.String, value,loc)
        {
            _value = value;
            Verbatim = verb;
        }
       

        public override object GetValue()
        {
            return _value;
        }
   
     
    }	
}