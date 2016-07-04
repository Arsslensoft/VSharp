namespace VSC.AST {

	public class Checked : Statement {
		public Block Block;
		
		public Checked (Block b, Location loc)
		{
			Block = b;
			b.Unchecked = false;
			this.loc = loc;
		}

	}

	

}