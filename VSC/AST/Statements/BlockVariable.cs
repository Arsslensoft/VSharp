using System.Collections.Generic;
namespace VSC.AST {

public class BlockVariable : Statement
	{
		Expression initializer;
		protected FullNamedExpression type_expr;
		protected LocalVariable li;
		protected List<BlockVariableDeclarator> declarators;

		public BlockVariable (FullNamedExpression type, LocalVariable li)
		{
			this.type_expr = type;
			this.li = li;
			this.loc = type_expr.Location;
		}

		protected BlockVariable (LocalVariable li)
		{
			this.li = li;
		}

		#region Properties

		public List<BlockVariableDeclarator> Declarators {
			get {
				return declarators;
			}
		}

		public Expression Initializer {
			get {
				return initializer;
			}
			set {
				initializer = value;
			}
		}

		public FullNamedExpression TypeExpression {
			get {
				return type_expr;
			}
		}

		public LocalVariable Variable {
			get {
				return li;
			}
		}

		#endregion
        public void AddDeclarator(BlockVariableDeclarator decl)
        {
            if (declarators == null)
                declarators = new List<BlockVariableDeclarator>();

            declarators.Add(decl);
        }
	}


}