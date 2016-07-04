namespace VSC.AST {
 public class Restrict : TryFinallyBlock
    {
        /*Expression expr;
        TemporaryVariableReference expr_copy;
      

        public Restrict(Expression expr, Statement stmt, Location loc)
            : base(stmt, loc)
        {
            this.expr = expr;
        }

        public Expression Expr
        {
            get
            {
                return this.expr;
            }
        }

        protected override bool DoFlowAnalysis(FlowAnalysisContext fc)
        {
            expr.FlowAnalysis(fc);
            return base.DoFlowAnalysis(fc);
        }

        public override bool Resolve(BlockContext ec)
        {
            expr = expr.Resolve(ec);
            if (expr == null)
                return false;


            if (!IsPredefinedTypeR(expr.Type, ec) && !(expr.Type.IsClass || expr.Type.IsStruct))
            {
                ec.Report.Error(185, loc,
                    "`{0}' is not a reference type as required by the restrict statement",
                    expr.Type.GetSignatureForError());
            }

            if (expr.Type.IsGenericParameter)
            {
                expr = Convert.ImplicitTypeParameterConversion(expr, (TypeParameterSpec)expr.Type, ec.BuiltinTypes.Object);
            }

            VariableReference lv = expr as VariableReference;
            bool locked;
            if (lv != null)
            {
                locked = lv.IsLockedByStatement;
                lv.IsLockedByStatement = false;
            }
            else
            {
                lv = null;
                locked = false;
            }

            //
            // Have to keep original lock value around to unlock same location
            // in the case of original value has changed or is null
            //
            if(IsPredefinedTypeR(expr.Type, ec))
                expr_copy = TemporaryVariableReference.Create(expr.Type, ec.CurrentBlock, loc);
            else
            expr_copy = TemporaryVariableReference.Create(ec.BuiltinTypes.Object, ec.CurrentBlock, loc);
           
            expr_copy.Resolve(ec);

       

            using (ec.Set(ResolveContext.Options.RestrictScope))
            {
                base.Resolve(ec);
            }

            if (lv != null)
            {
                lv.IsLockedByStatement = locked;
            }

            return true;
        }
        bool IsPredefinedTypeR(TypeSpec type, ResolveContext ec)
        {
            if ((type == ec.BuiltinTypes.Bool)
        || (type == ec.BuiltinTypes.Byte) || (type == ec.BuiltinTypes.SByte) || (type == ec.BuiltinTypes.Short) || (type == ec.BuiltinTypes.UShort) || (type == ec.BuiltinTypes.UInt) || (type == ec.BuiltinTypes.Int) || (type == ec.BuiltinTypes.Long) || (type == ec.BuiltinTypes.ULong) || (type == ec.BuiltinTypes.Char) || (type == ec.BuiltinTypes.Decimal) || (type == ec.BuiltinTypes.Float) || (type == ec.BuiltinTypes.Double) || (type == ec.BuiltinTypes.String) || (type == ec.BuiltinTypes.IntPtr) || (type == ec.BuiltinTypes.UIntPtr)
               || (type == ec.BuiltinTypes.Object))


                return true;
            else return false;




        }
        bool IsPredefinedType(TypeSpec type, EmitContext ec)
        {
            if ((type == ec.BuiltinTypes.Bool)
        || (type == ec.BuiltinTypes.Byte) || (type == ec.BuiltinTypes.SByte) || (type == ec.BuiltinTypes.Short) || (type == ec.BuiltinTypes.UShort) || (type == ec.BuiltinTypes.UInt) || (type == ec.BuiltinTypes.Int) || (type == ec.BuiltinTypes.Long) || (type == ec.BuiltinTypes.ULong) || (type == ec.BuiltinTypes.Char) || (type == ec.BuiltinTypes.Decimal) || (type == ec.BuiltinTypes.Float) || (type == ec.BuiltinTypes.Double) || (type == ec.BuiltinTypes.String) || (type == ec.BuiltinTypes.IntPtr) || (type == ec.BuiltinTypes.UIntPtr)
               || (type == ec.BuiltinTypes.Object))


                return true;
            else return false;




        }
        protected override void EmitTryBodyPrepare(EmitContext ec)
        {
            if (IsPredefinedType(expr.Type, ec))
            {

                // IS NOT A CLASS OR STRUCT
                expr_copy.EmitAssign(ec, expr);

              

            }
            else if ((expr.Type.IsClass || expr.Type.IsStruct) && (ec.Module.PredefinedMembers.CloneObject != null))
            {
                if (expr is FieldExpr)
                {
                    FieldExpr fld = expr as FieldExpr;
                    fld.Emit(ec);


                }
                else if (expr is PropertyExpr)
                {
                    PropertyExpr property = expr as PropertyExpr;
                    if (property.PropertyInfo.Set == null)
                        ec.Report.Error(9999, "Restrict : the property " + property.Name + " does not have a get accessor or does not exist");
                    else
                        property.Emit(ec);

                }
                else if (expr is LocalVariableReference)
                {
                    LocalVariableReference fld = expr as LocalVariableReference;
                    fld.Emit(ec);

                }
                else if (expr is ParameterReference)
                {
                    ParameterReference fld = expr as ParameterReference;
                    fld.Emit(ec);
                }
                else ec.Report.Error(9999, "Restrict statement supports only fields,property,local variable or parameter");

                
                ec.Emit(OpCodes.Call, ec.Module.PredefinedMembers.CloneObject.Resolve(
                   loc ));
                ec.Emit(OpCodes.Box, expr.Type);
                expr_copy.LocalInfo.CreateBuilder(ec);
                //  ec.Emit(OpCodes.Pop);
                ec.Emit(OpCodes.Stloc, expr_copy.LocalInfo.builder);
            }
            else ec.Report.Error(9999, "Restrict statement error");
            //else
            //{
            //    //
            //    // Monitor.Enter (expr_copy)
            //    //
            // expr_copy.Emit(ec);
            //    ec.Emit(OpCodes.Call, ec.Module.PredefinedMembers.MonitorEnter.Get());
            //}

            base.EmitTryBodyPrepare(ec);
        }

        protected override void EmitTryBody(EmitContext ec)
        {
     
            Statement.Emit(ec);
        }

        public override void EmitFinallyBody(EmitContext ec)
        {
        
            Label skip = ec.DefineLabel();

          


            if (IsPredefinedType(expr.Type, ec))
            {
                if (expr is FieldExpr)
                {
                    FieldExpr fld = expr as FieldExpr;
                    fld.EmitAssign(ec, expr_copy, false, false);
                }
                else if (expr is PropertyExpr)
                {
                    PropertyExpr property = expr as PropertyExpr;
                    if (property.PropertyInfo.Set == null)
                        ec.Report.Error(9999, "Restrict : the property " + property.Name + " does not have a get accessor or does not exist");
                    else
                        property.EmitAssign(ec, expr_copy, false, false);
                }
                else if (expr is LocalVariableReference)
                {
                    LocalVariableReference fld = expr as LocalVariableReference;
                    fld.EmitAssign(ec, expr_copy, false, false);

                }
                else if (expr is ParameterReference)
                {
                    ParameterReference fld = expr as ParameterReference;
                    fld.EmitAssign(ec, expr_copy, false, false);
                }
                else ec.Report.Error(9999, "Restrict statement supports only fields,property,local variable or parameter");
            }
            else
            {

                //    ec.Emit(OpCodes.Ldloc, expr_copy.LocalInfo.builder);
                if (expr is FieldExpr)
                {
                    FieldExpr fld = expr as FieldExpr;
                    fld.EmitAssign(ec, expr_copy, false, false);
                }
                else if (expr is PropertyExpr)
                {
                    PropertyExpr property = expr as PropertyExpr;
                    if (property.PropertyInfo.Set == null)
                        ec.Report.Error(9999, "Restrict : the property " + property.Name + " does not have a get accessor or does not exist");
                    else
                        property.EmitAssign(ec, expr_copy, false, false);

                }
                else if (expr is LocalVariableReference)
                {
                    LocalVariableReference fld = expr as LocalVariableReference;
                    fld.EmitAssign(ec, expr_copy, false, false);

                }
                else if (expr is ParameterReference)
                {
                    ParameterReference fld = expr as ParameterReference;
                    fld.EmitAssign(ec, expr_copy, false, false);
                }
                else ec.Report.Error(9999, "Restrict statement supports only fields,property,local variable or parameter");
            }
       

            ec.MarkLabel(skip);
        }

      

        protected override void CloneTo(CloneContext clonectx, Statement t)
        {
            Restrict target = (Restrict)t;

            target.expr = expr.Clone(clonectx);
            target.stmt = Statement.Clone(clonectx);
        }

        public override object Accept(StructuralVisitor visitor)
        {
            return visitor.Visit(this);
        }

 */   }

    

}