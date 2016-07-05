using System;
using VSC.TypeSystem;

namespace VSC.AST
{
    public class NullConstant : Constant
    {
        public NullConstant (Location loc)
            : base (loc)
        {
	
            this.type = KnownTypeReference.Object;
        }

	
		

        public override object GetValue ()
        {
            return null;
        }

        public override string GetValueAsLiteral()
        {
            return "null";
        }


        public override long GetValueAsLong ()
        {
            throw new NotSupportedException ();
        }

        public override bool IsDefaultValue {
            get { return true; }
        }

        public override bool IsNegative {
            get { return false; }
        }

        public override bool IsNull {
            get { return true; }
        }

        public override bool IsZeroInteger {
            get { return true; }
        }
    }
}