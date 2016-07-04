using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.TypeSystem;

namespace VSC.AST
{
    public interface ILiteralConstant
    {
		char[] ParsedValue { get; set; }
    }
 
    public class NullLiteral : NullConstant
    {
        //
        // Default type of null is an object
        //
        public NullLiteral(Location loc)
            : base(loc)
        {
        }

        public override string GetValueAsLiteral()
        {
            return "null";
        }

        public override bool IsLiteral
        {
            get { return true; }
        }

 
    }
    public class BoolLiteral : BoolConstant, ILiteralConstant
    {
        public BoolLiteral(bool val, Location loc)
            : base(val, loc)
        {
        }

        public override bool IsLiteral
        {
            get { return true; }
        }

		public char[] ParsedValue { get; set; }

    }
    public class CharLiteral : CharConstant, ILiteralConstant
    {
        public CharLiteral( char c, Location loc)
            : base(c, loc)
        {
        }

        public override bool IsLiteral
        {
            get { return true; }
        }

		public char[] ParsedValue { get; set; }

    }
    public class IntLiteral : IntConstant, ILiteralConstant
    {
        public IntLiteral(int l, Location loc)
            : base(l, loc)
        {
        }
        public override bool IsLiteral
        {
            get { return true; }
        }

		public char[] ParsedValue { get; set; }

    }
    public class UIntLiteral : UIntConstant, ILiteralConstant
    {
        public UIntLiteral(uint l, Location loc)
            : base(l, loc)
        {
        }

        public override bool IsLiteral
        {
            get { return true; }
        }

		public char[] ParsedValue { get; set; }
    }
    public class LongLiteral : LongConstant, ILiteralConstant
    {
        public LongLiteral( long l, Location loc)
            : base(l, loc)
        {
        }

        public override bool IsLiteral
        {
            get { return true; }
        }


		public char[] ParsedValue { get; set; }
    }
    public class ULongLiteral : ULongConstant, ILiteralConstant
    {
        public ULongLiteral(ulong l, Location loc)
            : base(l, loc)
        {
        }

        public override bool IsLiteral
        {
            get { return true; }
        }

		public char[] ParsedValue { get; set; }
    }
    public class FloatLiteral : FloatConstant, ILiteralConstant
    {
        public FloatLiteral( float f, Location loc)
            : base( f, loc)
        {
        }

        public override bool IsLiteral
        {
            get { return true; }
        }


		public char[] ParsedValue { get; set; }

    }
    public class DoubleLiteral : DoubleConstant, ILiteralConstant
    {
        public DoubleLiteral(double d, Location loc)
            : base(d, loc)
        {
        }

    
        public override bool IsLiteral
        {
            get { return true; }
        }

		public char[] ParsedValue { get; set; }

    }
    public class StringLiteral : StringConstant, ILiteralConstant
    {
        public StringLiteral(string s, Location loc)
            : base(s, loc)
        {
        }

        public override bool IsLiteral
        {
            get { return true; }
        }

		public char[] ParsedValue { get; set; }

    }
}
