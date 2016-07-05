namespace VSC.AST
{
    public sealed class RangeVariable : INamedBlockVariable
    {
        Block block;

        public RangeVariable(string name, Location loc)
        {
            Name = name;
            Location = loc;
        }

        #region Properties

        public Block Block
        {
            get
            {
                return block;
            }
            set
            {
                block = value;
            }
        }

        public bool IsDeclared
        {
            get
            {
                return true;
            }
        }

        public bool IsParameter
        {
            get
            {
                return false;
            }
        }

        public Location Location { get; private set; }

        public string Name { get; private set; }

        #endregion

       
    }
}