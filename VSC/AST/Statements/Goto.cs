namespace VSC.AST {

public class Goto : ExitStatement
	{
		string target;

		public Goto (string label, Location l)
		{
			loc = l;
			target = label;
		}

		public string Target {
			get { return target; }
		}

		protected override bool IsLocalExit {
			get {
				return true;
			}
		}

	
	}

}