namespace VSC.AST {
public class LabeledStatement : Statement {
		string name;
		Block block;
		
		public LabeledStatement (string name, Block block, Location l)
		{
			this.name = name;
			this.block = block;
			this.loc = l;
		}


		public Block Block {
			get {
				return block;
			}
		}

		public string Name {
			get { return name; }
		}

	}
	

}