namespace VSC.AST
{
    public class SimpleMemberName
    {
        public string Value;
        public Location Location;

        public SimpleMemberName(string name, Location loc)
        {
            this.Value = name;
            this.Location = loc;
        }
    }
}