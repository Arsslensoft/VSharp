using System;
namespace VSC.AST {
public class BlockVariableDeclarator
	{
		LocalVariable li;
		Expression initializer;

		public BlockVariableDeclarator (LocalVariable li, Expression initializer)
		{
			
			this.li = li;
			this.initializer = initializer;
		}

		#region Properties

		public LocalVariable Variable {
			get {
				return li;
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

		#endregion

	
	}



}