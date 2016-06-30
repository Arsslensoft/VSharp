using System;

using VSC.Base.GoldParser.Grammar;
using VSC.Base.GoldParser.Parser;

namespace VSC.Base.GoldParser.Semantic {
	public abstract class SemanticToken: IToken {
        public LineInfo position;
        protected Symbol symbol;

		protected internal virtual void Initialize(Symbol symbol, LineInfo position) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			this.symbol = symbol;
			this.position = position;
		}

        LineInfo IToken.Position
        {
			get {
				return position;
			}
		}

		bool IToken.NameIs(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			return (symbol != null) && name.Equals(symbol.Name, StringComparison.Ordinal);
		}

		Symbol IToken.Symbol {
			get {
				return symbol;
			}
		}
	}
}