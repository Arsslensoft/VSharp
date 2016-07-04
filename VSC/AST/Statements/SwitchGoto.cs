using VSC.Context;
namespace VSC.AST {

public abstract class SwitchGoto : Statement
	{
		protected Switch switch_statement;

		protected SwitchGoto (Location loc)
		{
			this.loc = loc;
		}


		protected void Error_GotoCaseRequiresSwitchBlock ()
		{
            CompilerContext.report.Error (153, loc, "A goto case is only valid inside a switch statement");
		}
	}
	
}