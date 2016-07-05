using System.Diagnostics;
using System.Text;

namespace VSC.AST
{
    [DebuggerDisplay("{GetSignatureForError()}")]
    public class MemberName
    {
        public static readonly MemberName Null = new MemberName("");

        public readonly string Name;
        public TypeParameters TypeParameters;
        public readonly FullNamedExpression ExplicitInterface;
        public readonly Location Location;

        public readonly MemberName Left;

        public MemberName(string name)
            : this(name, Location.Null)
        { }

        public MemberName(string name, Location loc)
            : this(null, name, loc)
        { }

        public MemberName(string name, TypeParameters tparams, Location loc)
        {
            this.Name = name;
            this.Location = loc;

            this.TypeParameters = tparams;
        }

        public MemberName(string name, TypeParameters tparams, FullNamedExpression explicitInterface, Location loc)
            : this(name, tparams, loc)
        {
            this.ExplicitInterface = explicitInterface;
        }

        public MemberName(MemberName left, string name, Location loc)
        {
            this.Name = name;
            this.Location = loc;
            this.Left = left;
        }

        public MemberName(MemberName left, string name, FullNamedExpression explicitInterface, Location loc)
            : this(left, name, loc)
        {
            this.ExplicitInterface = explicitInterface;
        }

        public MemberName(MemberName left, MemberName right)
        {
            this.Name = right.Name;
            this.Location = right.Location;
            this.TypeParameters = right.TypeParameters;
            this.Left = left;
        }

        public int Arity
        {
            get
            {
                return TypeParameters == null ? 0 : TypeParameters.Count;
            }
        }

        public bool IsGeneric
        {
            get
            {
                return TypeParameters != null;
            }
        }

        public string Basename
        {
            get
            {
                if (TypeParameters != null)
                    return MakeName(Name, TypeParameters);
                return Name;
            }
        }

        public void CreateMetadataName(StringBuilder sb)
        {
            if (Left != null)
                Left.CreateMetadataName(sb);

            if (sb.Length != 0)
            {
                sb.Append(".");
            }

            sb.Append(Basename);
        }

        public string GetSignatureForDocumentation()
        {
            var s = Basename;

            if (ExplicitInterface != null)
                s = ExplicitInterface.GetSignatureForError() + "." + s;

            if (Left == null)
                return s;

            return Left.GetSignatureForDocumentation() + "." + s;
        }

        public string GetSignatureForError()
        {
            string s = TypeParameters == null ? null : "<" + TypeParameters.GetSignatureForError() + ">";
            s = Name + s;

            if (ExplicitInterface != null)
                s = ExplicitInterface.GetSignatureForError() + "." + s;

            if (Left == null)
                return s;

            return Left.GetSignatureForError() + "." + s;
        }

        public override bool Equals(object other)
        {
            return Equals(other as MemberName);
        }

        public bool Equals(MemberName other)
        {
            if (this == other)
                return true;
            if (other == null || Name != other.Name)
                return false;

            if ((TypeParameters != null) &&
                (other.TypeParameters == null || TypeParameters.Count != other.TypeParameters.Count))
                return false;

            if ((TypeParameters == null) && (other.TypeParameters != null))
                return false;

            if (Left == null)
                return other.Left == null;

            return Left.Equals(other.Left);
        }

        public override int GetHashCode()
        {
            int hash = Name.GetHashCode();
            for (MemberName n = Left; n != null; n = n.Left)
                hash ^= n.Name.GetHashCode();

            if (TypeParameters != null)
                hash ^= TypeParameters.Count << 5;

            return hash & 0x7FFFFFFF;
        }

        public static string MakeName(string name, TypeParameters args)
        {
            if (args == null)
                return name;

            return name + "`" + args.Count;
        }


    }
}