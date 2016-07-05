using VSC.TypeSystem;

namespace VSC.AST
{
    public class UIntConstant : IntegralConstant {
        public readonly uint Value;

        public UIntConstant ( uint v, Location loc)
            : base (KnownTypeReference.UInt32, loc)
        {
            Value = v;
        }

	
        public override object GetValue ()
        {
            return Value;
        }

        public override long GetValueAsLong ()
        {
            return Value;
        }

        public override Constant Increment ()
        {
            return new UIntConstant (checked(Value + 1), loc);
        }
	
        public override bool IsDefaultValue {
            get {
                return Value == 0;
            }
        }

        public override bool IsNegative {
            get {
                return false;
            }
        }
		
        public override bool IsOneInteger {
            get {
                return Value == 1;
            }
        }		

        public override bool IsZeroInteger {
            get { return Value == 0; }
        }

	

    }
}