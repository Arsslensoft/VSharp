using System;
using System.Globalization;

namespace VSC.Base
{
	/// <summary>
	/// Holds 16 boolean values.
	/// </summary>
	[Serializable]
	[CLSCompliant(false)]
	public struct BitVector16 : IEquatable<BitVector16>
	{
		ushort data;
		
		public bool this[ushort mask] {
			get { return (data & mask) != 0; }
			set {
				if (value)
					data |= mask;
				else
					data &= unchecked((ushort)~mask);
			}
		}
		
		public ushort Data {
			get { return data; }
			set { data = value; }
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			if (obj is BitVector16)
				return Equals((BitVector16)obj); // use Equals method below
			else
				return false;
		}
		
		public bool Equals(BitVector16 other)
		{
			return this.data == other.data;
		}
		
		public override int GetHashCode()
		{
			return data;
		}
		
		public static bool operator ==(BitVector16 left, BitVector16 right)
		{
			return left.data == right.data;
		}
		
		public static bool operator !=(BitVector16 left, BitVector16 right)
		{
			return left.data != right.data;
		}
		#endregion
		
		public override string ToString()
		{
			return data.ToString("x4", CultureInfo.InvariantCulture);
		}
	}
}
