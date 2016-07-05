using System.Text;

namespace VSC.AST
{
    public class ComposedTypeSpecifier
    {
        public static readonly ComposedTypeSpecifier SingleDimension = new ComposedTypeSpecifier(1, Location.Null);

        public readonly int Dimension;
        public readonly Location Location;

        public ComposedTypeSpecifier(int specifier, Location loc)
        {
            this.Dimension = specifier;
            this.Location = loc;
        }

        #region Properties
        public bool IsNullable
        {
            get
            {
                return Dimension == -1;
            }
        }

        public bool IsPointer
        {
            get
            {
                return Dimension == -2;
            }
        }

        public ComposedTypeSpecifier Next { get; set; }

        #endregion

        public static ComposedTypeSpecifier CreateArrayDimension(int dimension, Location loc)
        {
            return new ComposedTypeSpecifier(dimension, loc);
        }

        public static ComposedTypeSpecifier CreateNullable(Location loc)
        {
            return new ComposedTypeSpecifier(-1, loc);
        }

        public static ComposedTypeSpecifier CreatePointer(Location loc)
        {
            return new ComposedTypeSpecifier(-2, loc);
        }

        public string GetSignatureForError()
        {
            string s =
                IsPointer ? "*" :
                    IsNullable ? "?" :
                        GetPostfixSignature(Dimension);

            return Next != null ? s + Next.GetSignatureForError() : s;
        }

        public static string GetPostfixSignature(int rank)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 1; i < rank; i++)
            {
                sb.Append(",");
            }
            sb.Append("]");

            return sb.ToString();
        }

    }
}