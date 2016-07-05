using VSC.TypeSystem;

namespace VSC.AST
{
    public class ShortConstant : IntegralConstant {
        public readonly short Value;

	

        public ShortConstant (short v, Location loc)
            : base (KnownTypeReference.Int16, loc)
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
            return new ShortConstant (checked((short)(Value + 1)), loc);
        }

        public override bool IsDefaultValue {
            get {
                return Value == 0;
            }
        }
		
        public override bool IsZeroInteger {
            get { return Value == 0; }
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

	
    }
}