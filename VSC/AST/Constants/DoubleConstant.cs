using System;
using VSC.TypeSystem;

namespace VSC.AST
{
    public class DoubleConstant : Constant
    {
        public readonly double Value;

	
        public DoubleConstant ( double v, Location loc)
            : base (loc)
        {
            this.type = KnownTypeReference.Double;
		

            Value = v;
        }



        public override object GetValue ()
        {
            return Value;
        }

        public override string GetValueAsLiteral ()
        {
            return Value.ToString ();
        }

        public override long GetValueAsLong ()
        {
            throw new NotSupportedException ();
        }

        public override bool IsDefaultValue {
            get {
                return Value == 0;
            }
        }

        public override bool IsNegative {
            get {
                return Value < 0;
            }
        }

	

    }
}