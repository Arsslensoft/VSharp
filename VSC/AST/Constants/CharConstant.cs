using VSC.TypeSystem;

namespace VSC.AST
{
    public class CharConstant : Constant {
        public readonly char Value;

        public CharConstant (char v, Location loc)
            : base (loc)
        {
            this.type = KnownTypeReference.Char;

            Value = v;
        }

		
        static string descape (char c)
        {
            switch (c){
                case '\a':
                    return "\\a"; 
                case '\b':
                    return "\\b"; 
                case '\n':
                    return "\\n"; 
                case '\t':
                    return "\\t"; 
                case '\v':
                    return "\\v"; 
                case '\r':
                    return "\\r"; 
                case '\\':
                    return "\\\\";
                case '\f':
                    return "\\f"; 
                case '\0':
                    return "\\0"; 
                case '"':
                    return "\\\""; 
                case '\'':
                    return "\\\'"; 
            }
            return c.ToString ();
        }

        public override object GetValue ()
        {
            return Value;
        }

        public override long GetValueAsLong ()
        {
            return Value;
        }

        public override string GetValueAsLiteral ()
        {
            return "\"" + descape (Value) + "\"";
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

        public override bool IsZeroInteger {
            get { return Value == '\0'; }
        }



    }
}