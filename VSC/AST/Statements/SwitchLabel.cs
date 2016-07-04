namespace VSC.AST {

public class SwitchLabel : Statement
	{
		Expression label;


		//
		// if expr == null, then it is the default case.
		//
		public SwitchLabel (Expression expr, Location l)
		{
			label = expr;
			loc = l;
		}

		public bool IsDefault {
			get {
				return label == null;
			}
		}

		public Expression Label {
			get {
				return label;
			}
		}

		public Location Location {
			get {
				return loc;
			}
		}
        public bool SectionStart { get; set; }
	}

}