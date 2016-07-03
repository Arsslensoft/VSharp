namespace VSC.AST {
 public class AsyncSt : Statement
    {


        /*AnonymousMethodExpression AnonymousME;
        AnonymousMethodBody AnonymousMB;


        public AsyncSt(AnonymousMethodExpression stmt, Location loc)
        {

            AnonymousME = stmt;

            this.loc = loc;
        }

        Expression es;
        public override bool Resolve(BlockContext ec)
        {


            AnonymousME = (AnonymousMethodExpression)AnonymousME.Resolve(ec);
            TypeSpec ts = ec.Module.PredefinedTypes.ThreadStartCallbackType.Resolve();
            es = AnonymousME.Compatible(ec, ts);
            AnonymousMB = (AnonymousMethodBody)es.Resolve(ec);


            return true;
        }



        protected override void DoEmit(EmitContext ec)
        {

            LocalBuilder sp = ec.DeclareLocal(ec.Module.PredefinedTypes.ThreadStartCallbackType.Resolve(), false);


            //// cURRENT
            //PropertySpec SyncCtx = ec.Module.PredefinedMembers.SyncContextCurrent.Resolve(loc);
            //ec.Emit(OpCodes.Call, SyncCtx.Get);
            //// STORE TO CURRENT
            //AssemblerGenerator.EmitStack(ec, sc.LocalIndex, false, false, true);



            // Emit MTD
            AnonymousMB.Emit(ec);
            //  // STORE TO SENDPOST CALLBACK
            AssemblerGenerator.EmitStack(ec, sp.LocalIndex, false, false, true);

            // invoke
            AssemblerGenerator.EmitStack(ec, sp.LocalIndex, false, false);

            ec.Emit(OpCodes.Ldnull);
            ec.Emit(OpCodes.Ldnull);

            MethodSpec SendPostCB = ec.Module.PredefinedMembers.ThreadStartCallBackBeginInvokeMethod.Resolve(loc);
            ec.Emit(OpCodes.Callvirt, SendPostCB);
            ec.Emit(OpCodes.Pop);
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
            AsyncSt target = (AsyncSt)t;

            target.AnonymousME.Block = (ParametersBlock)clonectx.LookupBlock((ParametersBlock)(AnonymousME.Block));
        }

        public override object Accept(StructuralVisitor visitor)
        {
            return visitor.Visit(this);
        }*/
    }
    


}