using System;
using VSC.TypeSystem.Resolver;
namespace VSC.AST
{
    public class ImportPackage :Import, IResolve,IAstNode
    {

        public ImportPackage(TypeNameExpression expr, Location loc)
            : base (expr, loc)
        {
           
        }
        public IAstNode ParentNode { get; set; }
        public override bool DoResolve(ResolveContext rc)
        {
     
            Expression tp = expr.DoResolve(rc);
            if (tp != null)
            {
                if (tp is AliasNamespace)
                {
                    rc.Report.Error(149, Location,
                        "An `import' directive can only be applied to namespaces but `{0}' denotes a type.",
                        expr.GetSignatureForError());
                    return false;
                }
                return true;
            }
            else rc.Report.Error(150, Location,
                     "The imported package or type '{0}' does not exist in the current context",
                     expr.GetSignatureForError());

            return false;
        }

        public virtual void AcceptVisitor(IVisitor visitor)
        {
            visitor.Visit(this); 
        }
    }
}