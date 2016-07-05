namespace VSC.AST
{
    /// <summary>
    ///   Represents the `self' construct
    /// </summary>
    public class SelfReference : VariableReference
    {

        public SelfReference(Location loc)
        {
            this.loc = loc;
        }

        public override string Name
        {
            get { return "self"; }
        }

      

    }
}