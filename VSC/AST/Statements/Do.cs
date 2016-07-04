namespace VSC.AST {
public class Do : LoopStatement
	{
		public Expression expr;
       
		public Do (Statement statement, BooleanExpression bool_expr, Location doLocation, Location whileLocation)
			: base (statement)
		{
			expr = bool_expr;
			loc = doLocation;
			WhileLocation = whileLocation;
		}
    
		public Location WhileLocation {
			get; private set;
		}


		
	}


}