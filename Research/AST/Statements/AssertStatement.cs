namespace VSC.AST {
public class AssertStatement : Statement
    {
      /*  Expression expr;
        Expression msg;
        public AssertStatement(Expression expr, Expression msg, Location l)
        {
            this.expr = expr;
            loc = l;
            this.msg = msg;
        }

        public Expression Expr
        {
            get
            {
                return this.expr;
            }
        }
        public Expression Message
        {
            get
            {
                return this.msg;
            }
        }

        public override bool Resolve(BlockContext ec)
        {

            expr = expr.Resolve(ec);
            msg = msg.Resolve(ec);
            if (expr == null || msg== null)
                return false;

            if (msg.Type != ec.BuiltinTypes.String)
                ec.Report.Error(3669, "Assertion message must be type of string");
            return true;
        }

        protected override void DoEmit(EmitContext ec)
        {
            Label false_target = ec.DefineLabel();
            expr.EmitBranchable(ec, false_target, true);

            msg.Emit(ec);
           MethodSpec m = ec.Module.PredefinedMembers.AssertionExceptionCtor.Resolve(loc);
            if (m != null)
                ec.Emit(OpCodes.Newobj, m);
            ec.Emit(OpCodes.Throw);

            ec.MarkLabel(false_target);
        }

        protected override bool DoFlowAnalysis(FlowAnalysisContext fc)
        {
            if (expr != null)
                expr.FlowAnalysis(fc);
            if (msg != null)
                msg.FlowAnalysis(fc);
            return true;
        }

        public override Reachability MarkReachable(Reachability rc)
        {
            return base.MarkReachable(rc);
            
        }

        protected override void CloneTo(CloneContext clonectx, Statement t)
        {
            AssertStatement target = (AssertStatement)t;

            if (expr != null)
                target.expr = expr.Clone(clonectx);
            if (msg != null)
                target.msg = msg.Clone(clonectx);
        }

        public override object Accept(StructuralVisitor visitor)
        {
            return visitor.Visit(this);
        }*/
    }
   


}