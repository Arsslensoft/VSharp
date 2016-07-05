using VSC.TypeSystem;

namespace VSC.AST
{
    public class ULongConstant : IntegralConstant {
        public readonly ulong Value;


        public ULongConstant (ulong v, Location loc)
            : base (KnownTypeReference.UInt64, loc)
        {
            Value = v;
        }

	
        public override object GetValue ()
        {
            return Value;
        }

        public override long GetValueAsLong ()
        {
            return (long) Value;
        }

        public override Constant Increment ()
        {
            return new ULongConstant (checked(Value + 1), loc);
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