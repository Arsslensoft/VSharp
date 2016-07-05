namespace VSC
{
    public class LocatedToken
    {
        public int row, column;
        public string value;
        public SourceFile file;

        public LocatedToken()
        {
        }

        public LocatedToken(string value, Location loc)
        {
            this.value = value;
            file = loc.SourceFile;
            row = loc.Line;
            column = loc.Column;
        }

        public override string ToString()
        {
            return string.Format("Token '{0}' at {1},{2}", Value, row, column);
        }

        public Location Location
        {
            get { return new Location(file, row, column); }
        }

        public string Value
        {
            get { return value; }
        }
    }
}