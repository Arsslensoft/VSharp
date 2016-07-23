using System.Collections.Generic;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class DeclarationExpression : Expression
    {
        LocalVariableExpression lvr;
        public DeclarationExpression(FullNamedExpression variableType, LocalVariable variable)
        {
            VariableType = variableType;
            Variable = variable;
            this.loc = variable.Location;
        }

        public LocalVariable Variable { get; set; }
        public Expression Initializer { get; set; }
        public FullNamedExpression VariableType { get; set; }

   
        bool DoResolveCommon(ResolveContext rc)
        {
            var var_expr = VariableType as VarTypeExpression;
            if (var_expr != null)
                ResolvedType = null;
            else
            {
                ResolvedType = VariableType.ResolveAsType(rc);
                if (ResolvedType == null)
                    return false;
            }

            Variable.type = ResolvedType;
           rc = rc.AddVariable(Variable);
            if (Initializer != null)
            {
                Initializer = Initializer.DoResolve(rc);

                if (var_expr != null && Initializer != null && var_expr.InferType(rc, Initializer)) 
                    ResolvedType = var_expr.Type;
             
            }

            lvr = new LocalVariableExpression(Variable, loc);

            eclass = ExprClass.Variable;
            return true;
        }
    
		
        public override Expression DoResolve(ResolveContext rc)
        {
            if (DoResolveCommon(rc))
                lvr.DoResolve(rc);

            return this;
        }
        public override Expression DoResolveLeftValue(ResolveContext rc, Expression right_side)
        {
            if (lvr == null && DoResolveCommon(rc))
                lvr.DoResolveLeftValue(rc, right_side);

            return this;
        }
    }
}