using System.Collections.Generic;
namespace VSC.AST {
public class TryCatch : ExceptionStatement
	{
		public Block Block;
		protected List<Except> clauses;
		protected readonly bool inside_try_finally;
		protected List<Except> catch_sm;

		public TryCatch (Block block, List<Except> catch_clauses, Location l, bool inside_try_finally)
			: base (l)
		{
			this.Block = block;
			this.clauses = catch_clauses;
			this.inside_try_finally = inside_try_finally;
		}

		public List<Except> Clauses {
			get {
				return clauses;
			}
		}

		public bool IsTryCatchFinally {
			get {
				return inside_try_finally;
			}
		}

	
	}

	

}