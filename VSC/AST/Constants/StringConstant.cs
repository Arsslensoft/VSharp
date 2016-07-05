using System;
using VSC.TypeSystem;

namespace VSC.AST
{
    public class StringConstant : Constant {
	
        public StringConstant (string s, Location loc)
            : base (loc)
        {
            this.type = KnownTypeReference.String;
	

            Value = s;
        }

        protected StringConstant (Location loc)
            : base (loc)
        {
        }

        public string Value { get; protected set; }

        public override object GetValue ()
        {
            return Value;
        }

        public override string GetValueAsLiteral ()
        {
            // FIXME: Escape the string.
            return "\"" + Value + "\"";
        }

        public override long GetValueAsLong ()
        {
            throw new NotSupportedException ();
        }
		
	
        public override bool IsDefaultValue {
            get {
                return Value == null;
            }
        }

        public override bool IsNegative {
            get {
                return false;
            }
        }

        public override bool IsNull {
            get {
                return IsDefaultValue;
            }
        }

	
    }
}