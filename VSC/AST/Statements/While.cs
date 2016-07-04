namespace VSC.AST {

public class While : LoopStatement
	{
		public Expression expr;
	
		
		public While (BooleanExpression bool_expr, Statement statement, Location l)
			: base (statement)
		{
			this.expr = bool_expr;
			loc = l;
		}

        public While(BooleanExpression bool_expr, Statement statement,Statement elsestmt,Location el, Location l)
            : base(statement,elsestmt)
        {
            this.expr = bool_expr;
            loc = l;
            ElseLocation = el;
        }
	
	}

}