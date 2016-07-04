namespace VSC.AST {
public class Safe : Statement
    {
      /*  public Block Block;
        public Expression expr;
        public Safe(Expression expr,Statement b, Location loc)
        {
            Block = (Block)b;
            Block.Unsafe = false;
            this.loc = loc;
            this.expr = expr;
        }
        public Safe(Block b, Location loc)
        {
            Block = b;
            Block.Unsafe = false;
            this.loc = loc;
        }
        bool ContainsThrow(Block b)
        {
            if (b.Statements.Count > 0)
            {
                for (int i = 0; i < b.Statements.Count; i++)
                {
                    if (b.Statements[i] is Throw)
                        return true;
                }

            }
            return false;
        }
        public override bool Resolve(BlockContext ec)
        {
            if (expr != null)
                expr = expr.Resolve(ec);
        
            
            if (ContainsThrow(Block))
                ec.Report.Error(9999, loc, "Safe main body must not include any throw instruction");

            if (ec.CurrentIterator != null)
                ec.Report.Error(1629, loc, "Safe code may not appear in iterators");

            using (ec.Set(ResolveContext.Options.SafeScope))
                return Block.Resolve(ec);
        }

        protected override void DoEmit(EmitContext ec)
        {
            Label TryCatchLabel = ec.ig.BeginExceptionBlock();
            LocalBuilder lb = ec.DeclareLocal(ec.BuiltinTypes.Exception, false);
            Block.Emit(ec);
            ec.Emit(OpCodes.Leave, TryCatchLabel);
            ec.BeginCatchBlock(ec.BuiltinTypes.Exception);
            ec.Emit(OpCodes.Stloc_S, lb);
            if (expr != null) // filter
            {
                Label false_target = ec.DefineLabel();

                Constant c = expr as Constant;
                if (c != null)
                {
                    c.EmitSideEffect(ec);

                    if (!c.IsDefaultValue)
                    {
                        // emit throw
                        ec.Emit(OpCodes.Ldloc_S, lb);
                        ec.Emit(OpCodes.Throw);
                    }


                }
                else
                {
                    expr.EmitBranchable(ec, false_target, false);
                    // emit throw
                    ec.Emit(OpCodes.Ldloc_S, lb);
                    ec.Emit(OpCodes.Throw);

                    ec.MarkLabel(false_target);
                }
            }
            ec.BeginFinallyBlock();

            Label start_finally = ec.DefineLabel();
            ec.MarkLabel(start_finally);

            ec.EndExceptionBlock();


        }

        protected override bool DoFlowAnalysis(FlowAnalysisContext fc)
        {
            return Block.FlowAnalysis(fc);
        }

        public override Reachability MarkReachable(Reachability rc)
        {
            base.MarkReachable(rc);
            return Block.MarkReachable(rc);
        }

        protected override void CloneTo(CloneContext clonectx, Statement t)
        {
            Safe target = (Safe)t;

            target.Block = clonectx.LookupBlock(Block);
        }

        public override object Accept(StructuralVisitor visitor)
        {
            return visitor.Visit(this);
        }*/
    }



}