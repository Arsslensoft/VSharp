using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

using VSC.Base.GoldParser.Grammar;

namespace VSC.Base.GoldParser.Parser {
	/// <summary>
	/// A reduction token, which contains the child tokens reduced with the <see cref="ParentRule"/>.
	/// </summary>
	public class Reduction: Token {
		private readonly Rule rule;
		private readonly ReadOnlyCollection<Token> tokens;

		internal Reduction(Rule rule, IList<Token> tokens): base() {
			if (rule == null) {
				throw new ArgumentNullException("rule");
			}
			if (tokens == null) {
				throw new ArgumentNullException("tokens");
			}
			this.rule = rule;
			Token[] tokenArray = new Token[tokens.Count];
			tokens.CopyTo(tokenArray, 0);
			this.tokens = Array.AsReadOnly(tokenArray);
		}

		public ReadOnlyCollection<Token> Children {
			[DebuggerStepThrough]
			get {
				return tokens;
			}
		}

		public override sealed LineInfo Position {
			[DebuggerStepThrough]
			get {
				return tokens.Count > 0 ? tokens[0].Position : default(LineInfo);
			}
		}

		public override sealed Symbol Symbol {
			[DebuggerStepThrough]
			get {
				return rule.RuleSymbol;
			}
		}

		public Rule Rule {
			get {
				return rule;
			}
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			foreach (Token token in tokens) {
				if (sb.Length > 0) {
					sb.Append(' ');
				}
				sb.Append(token);
			}
			return sb.ToString();
		}
	}
}