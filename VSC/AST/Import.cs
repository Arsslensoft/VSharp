using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class Import : IResolve
    {
       protected readonly TypeNameExpression expr;
       protected readonly Location loc;

        public Import(TypeNameExpression expr, Location loc)
        {
            this.expr = expr;
            this.loc = loc;
        }

        #region Properties

        public virtual AliasIdentifier Alias
        {
            get
            {
                return null;
            }
        }

        public Location Location
        {
            get
            {
                return loc;
            }
        }

        public TypeNameExpression NamespaceExpression
        {
            get
            {
                return expr;
            }
        }

   

        #endregion

        public virtual bool DoResolve(ResolveContext rc)
        {
            return false;
        }
        public string GetSignatureForError()
        {
            return expr.GetSignatureForError();
        }
      
     
    }
}