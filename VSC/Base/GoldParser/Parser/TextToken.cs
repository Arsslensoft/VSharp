using System;

using VSC.Base.GoldParser.Grammar;

namespace VSC.Base.GoldParser.Parser {
	/// <summary>
	/// Represents data about current token.
	/// </summary>
	public sealed class TextToken: Token {
		private readonly LineInfo position; // Token source line number.
		private readonly Symbol symbol; // Token symbol.
		private readonly string text; // Token text.

		internal TextToken(Symbol symbol, LineInfo position, string text) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			if (text == null) {
				throw new ArgumentNullException("text");
			}
			this.symbol = symbol;
			this.position = position;
			this.text = this.symbol.Name.Equals(text, StringComparison.Ordinal) ? this.symbol.Name : text; // "intern" short strings which are equal to the terminal name
		}

		public override LineInfo Position {
			get {
				return position;
			}
		}

		public override Symbol Symbol {
			get {
				return symbol;
			}
		}

		public override string Text {
			get {
				return text;
			}
		}

		public override string ToString() {
			return text;
		}
	}
}