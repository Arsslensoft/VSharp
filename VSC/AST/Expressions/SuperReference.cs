namespace VSC.AST
{
    public class SuperReference : SelfReference
    {
        public SuperReference(Location loc)
            : base(loc)
        {
        }
        public override string Name
        {
            get
            {
                return "super";
            }
        }

    }
}