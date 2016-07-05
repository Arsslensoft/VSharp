namespace VSC.AST
{
    public class YieldBreak : ExitStatement
    {


        public YieldBreak(Location l)
        {
            loc = l;
        }

        protected override bool IsLocalExit
        {
            get
            {
                return false;
            }
        }
    }
}