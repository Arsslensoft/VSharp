using VSC.TypeSystem;

namespace VSC.AST
{
    public class SByteConstant : IntegralConstant
    {
        public readonly sbyte Value;


        public SByteConstant ( sbyte v, Location loc)
            : base (KnownTypeReference.SByte, loc)
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
            return new SByteConstant (checked((sbyte)(Value + 1)), loc);
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