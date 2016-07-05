using VSC.TypeSystem;

namespace VSC.AST
{
    public class BoolConstant : Constant {
        public readonly bool Value;

		
        public BoolConstant ( bool val, Location loc)
            : base (loc)
        {
            this.type = KnownTypeReference.Boolean;

            Value = val;
        }

        public override object GetValue ()
        {
            return (object) Value;
        }

        public override string GetValueAsLiteral ()
        {
            return Value ? "true" : "false";
        }
        public override long GetValueAsLong ()
        {
            return Value ? 1 : 0;
        }
        public override bool IsDefaultValue {
            get {
                return !Value;
            }
        }

        public override bool IsNegative {
            get {
                return false;
            }
        }
	
        public override bool IsZeroInteger {
            get { return Value == false; }
        }

    }
}