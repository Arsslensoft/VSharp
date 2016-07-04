namespace VSC.AST {

	public class Unchecked : Statement {
		public Block Block;
		
		public Unchecked (Block b, Location loc)
		{
			Block = b;
			b.Unchecked = true;
			this.loc = loc;
		}

	}


}