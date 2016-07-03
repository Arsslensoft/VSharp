namespace VSC.AST {

 public class Loop : LoopStatement
    {
      /*  public Block Block;
        Expression exp;
        public Expression Expr { get { return exp; } }

        TemporaryVariableReference expr_copy;

        public Loop(Block b, Location loc)
            : base((Statement)b)
        {
            Block = b;
            Block.Unsafe = false;
            this.loc = loc;

            exp = null;
        }

        public Loop(Block b, Location loc, Expression expr)
            : base((Statement)b)
        {
            Block = b;
            Block.Unsafe = false;
            this.loc = loc;

            exp = expr;
        }
        bool ContainsLeave(Block b)
        {
            if (b.Statements.Count > 0)
            {
                for (int i = 0; i < b.Statements.Count; i++)
                {
                    if (b.Statements[i] is Break)
                        return true;
                }

            }
            return false;
        }
        bool IsPredefinedType(TypeSpec type, ResolveContext ec)
        {
            if ((type == ec.BuiltinTypes.Byte) || (type == ec.BuiltinTypes.SByte) || (type == ec.BuiltinTypes.Short) || (type == ec.BuiltinTypes.UShort) || (type == ec.BuiltinTypes.UInt) || (type == ec.BuiltinTypes.Int) || (type == ec.BuiltinTypes.Long) || (type == ec.BuiltinTypes.ULong))


                return true;
            else return false;




        }
        public override bool Resolve(BlockContext ec)
        {

            if (ContainsLeave(Block))
                ec.Report.Error(9999, loc, "Loop main body must not include any leave instruction");

            if (ec.CurrentIterator != null)
                ec.Report.Error(1629, loc, "Loop code may not appear in iterators");

            ec.EnclosingLoopOrSwitch = this;

            if (Expr != null)
            {
                exp = Expr.Resolve(ec);
                // Optimization
                if ((Expr is Constant) && GetConstSize((Expr as Constant)) == 0)
                    ec.Report.Error(1629, loc, "Loop expression supports only Int and Long types");
                else if (!IsPredefinedType(Expr.Type, ec))
                    ec.Report.Error(1629, loc, "Loop variables supports only Int,Double and Float types");
                else
                {

                    if (Expr is ParameterReference)
                    {
                        ParameterReference fld = Expr as ParameterReference;

                        if (fld.IsRef)
                            ec.Report.Error(1629, loc, "Loop does not support reference parameters");
                    }
                    if (exp.Type.IsGenericParameter)
                        exp = Convert.ImplicitTypeParameterConversion(exp, (TypeParameterSpec)exp.Type, ec.BuiltinTypes.Object);


                    expr_copy = TemporaryVariableReference.Create(Expr.Type, ec.CurrentBlock, loc);
                    expr_copy.Resolve(ec);
                }


            }
            using (ec.Set(ResolveContext.Options.LoopScope))
                return  base.Resolve(ec);


        }
        public object GetValueOfConst(Constant cnst)
        {
            var bytec = cnst as ByteConstant;
            var sbytec = cnst as SByteConstant;
            var intc = cnst as IntConstant;
            var uintc = cnst as UIntConstant;
            var shortc = cnst as ShortConstant;
            var ushortc = cnst as UShortConstant;
            var longc = cnst as LongConstant;
            var ulongc = cnst as ULongConstant;


            if (intc != null)
                return (uint)intc.Value;
            else if (uintc != null)
                return (uint)uintc.Value;
            else if (shortc != null)
                return (uint)shortc.Value;
            else if (ushortc != null)
                return (uint)ushortc.Value;
            else if (bytec != null)
                return (uint)bytec.Value;
            else if (sbytec != null)
                return (uint)sbytec.Value;

            else if (ulongc != null)
                return (ulong)ulongc.Value;
            else if (longc != null)
                return (ulong)longc.Value;
            else return 0;
        }
        public void EmitConst(Constant cnst, EmitContext ec)
        {

            // CHECK VALUE (INT OR LONG)
            if (cnst is ULongConstant || cnst is LongConstant)
                //Emit long
                ec.ig.Emit(OpCodes.Ldc_I8, (ulong)GetValueOfConst(cnst));
            else //Emit int
                ec.ig.Emit(OpCodes.Ldc_I4_S, (uint)GetValueOfConst(cnst));
        }
        public sbyte GetConstSize(Constant cnst)
        {
            if (cnst is IntConstant)
                return 4;
            else if (cnst is LongConstant)
                return 8;
            else if (cnst is ULongConstant)
                return 8;
            else if (cnst is UIntConstant)
                return 4;
            else if (cnst is ShortConstant)
                return 4;
            else if (cnst is UShortConstant)
                return 4;
            else if (cnst is ByteConstant)
                return 4;
            else if (cnst is SByteConstant)
                return 4;
            else
                return 0;
        }
        public TypeSpec GetLoopIteratorType(Expression expr, EmitContext ec)
        {

            if (expr is Constant)
            {
                int v = GetConstSize(expr as Constant);
                if (v == 4)
                    return ec.BuiltinTypes.UInt;
                else if (v == 8)
                    return ec.BuiltinTypes.ULong;

                else return null;

            }
            else return Expr.Type;
        }
        void EmitFromStack(EmitContext ec)
        {
            if (Expr is FieldExpr)
            {
                FieldExpr fld = Expr as FieldExpr;
                fld.EmitAssignFromStack(ec);
            }
            else if (Expr is PropertyExpr)
            {
                PropertyExpr property = Expr as PropertyExpr;

                ec.Emit(OpCodes.Call, property.PropertyInfo.Set);
            }
            else if (Expr is LocalVariableReference)
            {
                LocalVariableReference fld = Expr as LocalVariableReference;
                AssemblerGenerator.EmitStack(ec, fld.local_info.builder.LocalIndex, false, false, true);

            }
            else if (Expr is ParameterReference)
            {
                ParameterReference fld = Expr as ParameterReference;
                AssemblerGenerator.EmitStack(ec, fld.Parameter.Index, true, false, true);
            }
            else ec.Report.Error(9999, "Restrict statement supports only fields,property,local variable or parameter");


        }
        void EmitConvert(EmitContext ec)
        {
            if (Expr.Type == ec.BuiltinTypes.Long || Expr.Type == ec.BuiltinTypes.ULong)
                ec.Emit(OpCodes.Conv_I8);
            else if (Expr.Type == ec.BuiltinTypes.UShort || Expr.Type == ec.BuiltinTypes.Short)
                ec.Emit(OpCodes.Conv_I2);
            else if (Expr.Type == ec.BuiltinTypes.Byte || Expr.Type == ec.BuiltinTypes.SByte)
                ec.Emit(OpCodes.Conv_I1);
        }
        void EmitForLoop(EmitContext ec)
        {
            if (Expr is Constant)
            {
                TypeSpec tp = GetLoopIteratorType(Expr, ec); ;


                LocalBuilder ctr = ec.DeclareLocal(tp, false);

                // INIT CTR=0
                ec.Emit(OpCodes.Ldc_I4_0);


                // ASSIGN 
                ec.Emit(OpCodes.Stloc, ctr);

                // END CASTING

                // START LOOP
                Label old_begin = ec.LoopBegin;
                Label old_end = ec.LoopEnd;

                ec.LoopBegin = ec.DefineLabel();
                ec.LoopEnd = ec.DefineLabel();

                // expr is 'true', since the 'empty' case above handles the 'false' case
                ec.MarkLabel(ec.LoopBegin);


                if (ec.EmitAccurateDebugInfo)
                    ec.Emit(OpCodes.Nop);




                // Emit Block
                Block.Emit(ec);


                // INC I
                ec.Emit(OpCodes.Ldloc, ctr);
                ec.Emit(OpCodes.Ldc_I4_1);
                if (ec.Module.Compiler.Settings.Checked)
                    ec.Emit(OpCodes.Add_Ovf_Un);
                else ec.Emit(OpCodes.Addition);
                ec.Emit(OpCodes.Stloc, ctr);

                // PUSH VALS FOR EVAL
                ec.Emit(OpCodes.Ldloc, ctr);


                // Push Expr
                EmitConst(Expr as Constant, ec);



                //COMPARE
                ec.Emit(OpCodes.Blt_Un_S, ec.LoopBegin);

                //
                // Inform that we are infinite (ie, `we return'), only
                // if we do not `break' inside the code.
                //
                ec.MarkLabel(ec.LoopEnd);
            }
            else if (Expr != null)
            {

                // backup THE VARIABLE

                expr_copy.EmitAssign(ec, Expr);
                // INIT CTR=0
                ec.Emit(OpCodes.Ldc_I4_0);
                // ASSIGN 
                EmitFromStack(ec);
                // END CASTING

                // START LOOP
                Label old_begin = ec.LoopBegin;
                Label old_end = ec.LoopEnd;

                ec.LoopBegin = ec.DefineLabel();
                ec.LoopEnd = ec.DefineLabel();

                // expr is 'true', since the 'empty' case above handles the 'false' case
                ec.MarkLabel(ec.LoopBegin);


                //if (ec.EmitAccurateDebugInfo)
                //    ec.Emit(OpCodes.Nop);




                // Emit Block
                Block.Emit(ec);


                // INC I
                Expr.Emit(ec);
                // ec.Emit(OpCodes.Ldloc, ctr);


                ec.Emit(OpCodes.Ldc_I4_1);
                EmitConvert(ec);

                if (ec.Module.Compiler.Settings.Checked)
                    ec.Emit(OpCodes.Add_Ovf_Un);
                else ec.Emit(OpCodes.Addition);


                // STORE DATA In EXPR
                EmitFromStack(ec);







                // Push Expr
                Expr.Emit(ec);

                // PUSH VALS FOR EVAL
                expr_copy.Emit(ec);

                //COMPARE
                ec.Emit(OpCodes.Blt_Un_S, ec.LoopBegin);

                //
                // Inform that we are infinite (ie, `we return'), only
                // if we do not `break' inside the code.
                //
                ec.MarkLabel(ec.LoopEnd);

                // RESTORE VARIABLE
                if (Expr is FieldExpr)
                {
                    FieldExpr fld = Expr as FieldExpr;
                    fld.EmitAssign(ec, expr_copy, false, false);
                }
                else if (Expr is PropertyExpr)
                {
                    PropertyExpr property = Expr as PropertyExpr;
                    if (property.PropertyInfo.Set == null)
                        ec.Report.Error(9999, "Restrict : the property " + property.Name + " does not have a get accessor or does not exist");
                    else
                    {
                        expr_copy.Emit(ec);
                        ec.Emit(OpCodes.Call, property.PropertyInfo.Set);
                    }
                }
                else if (Expr is LocalVariableReference)
                {
                    LocalVariableReference fld = Expr as LocalVariableReference;
                    fld.EmitAssign(ec, expr_copy, false, false);

                }
                else if (Expr is ParameterReference)
                {
                    ParameterReference fld = Expr as ParameterReference;
                    fld.EmitAssign(ec, expr_copy, false, false);
                }
                else ec.Report.Error(9999, "Restrict statement supports only fields,property,local variable or parameter");
            }
            else
            {
                BoolConstant expr = new BoolConstant(ec.BuiltinTypes.Bool, true, loc);
                Label old_begin = ec.LoopBegin;
                Label old_end = ec.LoopEnd;

                ec.LoopBegin = ec.DefineLabel();
                ec.LoopEnd = ec.DefineLabel();

                // expr is 'true', since the 'empty' case above handles the 'false' case
                ec.MarkLabel(ec.LoopBegin);

                if (ec.EmitAccurateDebugInfo)
                    ec.Emit(OpCodes.Nop);

                expr.EmitSideEffect(ec);
                Block.Emit(ec);

                ec.Emit(OpCodes.Br, ec.LoopBegin);

                //
                // Inform that we are infinite (ie, `we return'), only
                // if we do not `break' inside the code.
                //
                ec.MarkLabel(ec.LoopEnd);
            }
        }

        protected override void DoEmit(EmitContext ec)
        {
            EmitForLoop(ec);
        }

        protected override bool DoFlowAnalysis(FlowAnalysisContext fc)
        {
            return Block.FlowAnalysis(fc);
        }

        public override Reachability MarkReachable(Reachability rc)
        {
            base.MarkReachable(rc);
            //  if(Expr == null)
            return Block.MarkReachable(rc);
            //else 
            //    return Reachability.CreateUnreachable();

        }

        protected override void CloneTo(CloneContext clonectx, Statement t)
        {
            Loop target = (Loop)t;

            target.Block = clonectx.LookupBlock(Block);
        }

        public override object Accept(StructuralVisitor visitor)
        {
            return visitor.Visit(this);
        }*/
    }


}