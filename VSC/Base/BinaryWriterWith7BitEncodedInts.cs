using System;
using System.IO;

namespace VSC.Base
{
    /// <summary>
    /// A binary writer that encodes all integers as 7-bit-encoded-ints.
    /// </summary>
    public sealed class BinaryWriterWith7BitEncodedInts : BinaryWriter
    {
        public BinaryWriterWith7BitEncodedInts(Stream stream) : base(stream)
        {
        }
		
        public override void Write(short value)
        {
            base.Write7BitEncodedInt(unchecked((ushort)value));
        }
		
        [CLSCompliant(false)]
        public override void Write(ushort value)
        {
            base.Write7BitEncodedInt(value);
        }
		
        public override void Write(int value)
        {
            base.Write7BitEncodedInt(value);
        }
		
        [CLSCompliant(false)]
        public override void Write(uint value)
        {
            base.Write7BitEncodedInt(unchecked((int)value));
        }
		
        public override void Write(long value)
        {
            this.Write(unchecked((ulong)value));
        }
		
        [CLSCompliant(false)]
        public override void Write(ulong value)
        {
            while (value >= 128) {
                this.Write(unchecked((byte)(value | 128u)));
                value >>= 7;
            }
            this.Write(unchecked((byte)value));
        }
    }
}