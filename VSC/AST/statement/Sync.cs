namespace VSC.AST {
public class Sync : Statement
    {
/*
        Expression expr;
        AnonymousMethodExpression AnonymousME;
        AnonymousMethodBody AnonymousMB;


        public Sync(Expression exp, AnonymousMethodExpression stmt, Location loc)
        {
            AsyncCallback asc;
            AnonymousME = stmt;
            AnonymousME.Block.Parameters = null;

            this.expr = exp;
            this.loc = loc;
        }
        public Expression Expr
        {
            get
            {
                return this.expr;
            }
        }
        Expression es;
        public override bool Resolve(BlockContext ec)
        {
            TypeExpression t = new TypeExpression(ec.BuiltinTypes.Object, loc);
            AnonymousME.Block.Parameters = new ParametersCompiled(new Parameter(t, "context", Parameter.Modifier.NONE, null, loc));
            AnonymousME = (AnonymousMethodExpression)AnonymousME.Resolve(ec);
            TypeSpec ts = ec.Module.PredefinedTypes.SendOrPostCallbackType.Resolve();
            es = AnonymousME.Compatible(ec, ts);
            AnonymousMB = (AnonymousMethodBody)es.Resolve(ec);

            expr = expr.Resolve(ec);
            if (expr == null)
                return false;
            if (ec.Module.PredefinedTypes.SynchronizationContextType.TypeSpec == null)
                ec.Module.PredefinedMembers.SyncContextRootMethod.Resolve(loc);

            if (expr.Type != ec.Module.PredefinedTypes.SynchronizationContextType.TypeSpec)
            {
                ec.Report.Error(185, loc,
                    "`{0}' is not a reference type as required by the sync statement [System.Threading.SynchronizationContext is required]",
                    expr.Type.GetSignatureForError());
            }


            return true;
        }



        protected override void DoEmit(EmitContext ec)
        {
            LocalBuilder sc = ec.DeclareLocal(ec.Module.PredefinedTypes.SynchronizationContextType.Resolve(), false);
            LocalBuilder sp = ec.DeclareLocal(ec.Module.PredefinedTypes.SendOrPostCallbackType.Resolve(), false);


            //// cURRENT
            //PropertySpec SyncCtx = ec.Module.PredefinedMembers.SyncContextCurrent.Resolve(loc);
            //ec.Emit(OpCodes.Call, SyncCtx.Get);
            //// STORE TO CURRENT
            //AssemblerGenerator.EmitStack(ec, sc.LocalIndex, false, false, true);

            // STORE TO CURRENT
            expr.Emit(ec);
            AssemblerGenerator.EmitStack(ec, sc.LocalIndex, false, false, true);

            // Emit MTD
            AnonymousMB.Emit(ec);
            //  // STORE TO SENDPOST CALLBACK
            AssemblerGenerator.EmitStack(ec, sp.LocalIndex, false, false, true);



            Label CTXNULL = ec.DefineLabel();

            AssemblerGenerator.EmitStack(ec, sc.LocalIndex, false, false);
            ec.Emit(OpCodes.Brfalse_S, CTXNULL);

            // Push args Call On CND

            AssemblerGenerator.EmitStack(ec, sc.LocalIndex, false, false);
            AssemblerGenerator.EmitStack(ec, sp.LocalIndex, false, false);

            ec.Emit(OpCodes.Ldnull);


            MethodSpec SyncPost = ec.Module.PredefinedMembers.SyncContextPostMethod.Resolve(loc);
            ec.Emit(OpCodes.Callvirt, SyncPost);
            ec.MarkLabel(CTXNULL);
            // invoke
            AssemblerGenerator.EmitStack(ec, sp.LocalIndex, false, false);
            ec.Emit(OpCodes.Ldnull);

            MethodSpec SendPostCB = ec.Module.PredefinedMembers.SendPostCallBackInvokeMethod.Resolve(loc);
            ec.Emit(OpCodes.Callvirt, SendPostCB);

        }

        protected override bool DoFlowAnalysis(FlowAnalysisContext fc)
        {
            return AnonymousME.Block.FlowAnalysis(fc);
        }

        public override Reachability MarkReachable(Reachability rc)
        {
            return base.MarkReachable(rc);

        }

        protected override void CloneTo(CloneContext clonectx, Statement t)
        {
            Sync target = (Sync)t;

            target.AnonymousME.Block = (ParametersBlock)clonectx.LookupBlock((ParametersBlock)(AnonymousME.Block));
        }

        public override object Accept(StructuralVisitor visitor)
        {
            return visitor.Visit(this);
        }
 */   }
   

}