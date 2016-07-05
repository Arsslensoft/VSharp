using System;
using VSC.TypeSystem;

namespace VSC.AST
{
    public class FloatConstant : Constant {
        //
        // Store constant value as double because float constant operations
        // need to work on double value to match JIT
        //
        public readonly double DoubleValue;

	
        public FloatConstant (double v, Location loc)
            : base (loc)
        {
            this.type = KnownTypeReference.Single;


            DoubleValue = v;
        }

	
        public float Value {
            get {
                return (float) DoubleValue;
            }
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