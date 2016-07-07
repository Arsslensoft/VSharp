namespace VSC.AST {

public class GroupAssign : Statement
    {
     /*   public List<Expression> Right { get; set; }
        public List<Expression> Left { get; set; }
     
        public GroupAssign(Location loc)
        {
            Right = null;
            Left = null;
       
            this.loc = loc;
        }
        public GroupAssign(List<Expression> left, List<Expression> right, Location loc)
        {
            Right = right;
            Left = left;
          
            this.loc = loc;
        }
        bool IsValidAssign(BlockContext ec)
        {
            if (Right.Count != Left.Count)
            {
                ec.Report.Error(3662,string.Format("Multiple assign count mismatch, {1} = {0} expected",Right.Count,Left.Count));
                return false;
            }
         
            foreach (Expression expr in Left)
                if (expr is Constant)
                {
                    ec.Report.Error(3663,"Left expression cannot be constant");
                    return false;
                }
            


            return true;
        }
        public override bool ResolveScope(BlockContext ec)
        {
            List<Expression> right = new List<Expression>();
            List<Expression> left = new List<Expression>();
     
            if (Right == null)
            {
                ec.Report.Error(9999, loc, "Undefined right expressions ");
                return false;
            }
            if (Left== null)
            {
                ec.Report.Error(9999, loc, "Undefined left expressions ");
                return false;
            }
            if (!IsValidAssign(ec))
            {
              
                return false;
            }
           

            foreach (Expression expr in Left)
                left.Addition(expr.ResolveScope(ec));

            foreach (Expression expr in Right)
                right.Addition(expr.ResolveScope(ec));


            for (int i = 0; i < right.Count; i++)
            {

                if (right[i] == null)
                {
                    ec.Report.Error(9999, loc, "Undefined right expression at "+i.ToString());
                    return false;
                }
                if (left[i] == null)
                {
                    ec.Report.Error(9999, loc, "Undefined left expression at " + i.ToString());
                    return false;
                }
          
              
               Expression tmp = new SimpleAssign(left[i], right[i]).ResolveScope(ec);
                if(tmp == null)
                    ec.Report.Error(9999, loc, string.Format("Incompatible types (try to cast) {0} = {1} ", left[i].Type, right[i].Type) + i.ToString());
           /*     if (right[i].Type != left[i].Type && (tmp = ResolveConversions(left[i], right[i], ec)) == null)
                {
                    ec.Report.Error(9999, loc, string.Format("Incompatible types (try to cast) {0} = {1} ", left[i].Type, right[i].Type) + i.ToString());
                    return false;
                }
                else if (tmp != null)
                    left[i] = tmp;
                *//*

            }
            Right = right;
            Left = left;
            return true;
        }
        Expression ResolveConversions(Expression source, Expression target,ResolveContext ec)
        {
        //  return Convert.ImplicitConversionRequired(ec, source, target.Type, source.Location);
            //
            // LAMESPEC: Under dynamic context no target conversion is happening
            // This allows more natual dynamic behaviour but breaks compatibility
            // with static binding
            //
            if (target is RuntimeValueExpression)
                return source;

            TypeSpec target_type = target.Type;

            //
            // 1. the return type is implicitly convertible to the type of target
            //
            if (Convert.ImplicitConversionExists(ec, source, target_type))
            {
                source = Convert.ImplicitConversion(ec, source, target_type, loc);
                return source;
            }

       

          
            if (source.Type.BuiltinType == BuiltinTypeSpec.Type.Dynamic)
            {
                ec.Report.Error(9999, "Dynamic type not supported by group assign");
                return null;
            }

            source.Error_ValueCannotBeConverted(ec, target_type, false);
            return null;
        }

        protected override void DoEmit(EmitContext ec)
        {
            foreach (Expression expr in Right)
                expr.Emit(ec);

            for(int i = Left.Count - 1; i >= 0; i--)
               Left[i].EmitAssignFromStack(ec);


        }

        protected override bool DoFlowAnalysis(FlowAnalysisContext fc)
        {
            return true ;
        }

        public override Reachability MarkReachable(Reachability rc)
        {
         return   base.MarkReachable(rc);
           
        }

        protected override void CloneTo(CloneContext clonectx, Statement t)
        {
            GroupAssign target = (GroupAssign)t;

            target.Right = Right;
            target.Left = Left;
        }

        public override object Accept(StructuralVisitor visitor)
        {
            return visitor.Visit(this);
        }
 */   }


}