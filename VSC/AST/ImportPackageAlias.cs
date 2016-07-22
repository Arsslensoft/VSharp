using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class ImportPackageAlias : ImportPackage
    {
        readonly AliasIdentifier alias;

        public ImportPackageAlias(AliasIdentifier alias, TypeNameExpression expr, Location loc)
            : base (expr, loc)
        {
            this.alias = alias;
        }

        public override AliasIdentifier Alias {
            get {
                return alias;
            }
        }
   

        public override bool DoResolve(ResolveContext rc)
        {
            Expression tp = expr.DoResolve(rc);

            if(   tp == null )
                rc.Report.Error(150, Location,
                     "The imported package or type '{0}' does not exist in the current context",
                     expr.GetSignatureForError());


            return true;
        }

        public override void AcceptVisitor(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}