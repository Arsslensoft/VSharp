using System;

using VSC.Base.GoldParser.Grammar;

namespace VSC.Base.GoldParser.Parser {
	/// <summary>
	/// The <c>Token</c> class is the non-generic base class for tokens in the AST (abstract syntax tree).
	/// </summary>
	public abstract class Token: IToken {
		public virtual string Text {
			get {
				return string.Empty;
			}
		}

		/// <summary>
		/// Gets the symbol associated with this token.
		/// </summary>
		/// <value>The parent symbol.</value>
		public abstract Symbol Symbol {
			get;
		}

		/// <summary>
		/// Gets the line number where this token begins.
		/// </summary>
		/// <value>The line number and position.</value>
		public abstract LineInfo Position {
			get;
		}

		public bool NameIs(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			Symbol symbol = Symbol;
			return (symbol != null) && name.Equals(symbol.Name, StringComparison.Ordinal);
		}
	}
}