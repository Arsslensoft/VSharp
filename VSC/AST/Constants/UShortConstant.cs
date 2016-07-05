using VSC.TypeSystem;

namespace VSC.AST
{
    public class UShortConstant : IntegralConstant
    {
        public readonly ushort Value;

        public UShortConstant ( ushort v, Location loc)
            : base (KnownTypeReference.UInt16, loc)
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
            return new UShortConstant ( checked((ushort)(Value + 1)), loc);
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