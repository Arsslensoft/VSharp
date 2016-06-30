using System;
using System.Text.RegularExpressions;

using VSC.Base.GoldParser.Grammar;

namespace VSC.Base.GoldParser.Semantic {
	/// <summary>
	/// This class is used to decorate constructors which accept exactly one string for the terminal value
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class TerminalAttribute: Attribute {
		private static readonly Regex rxSpecialToken = new Regex(@"^\(.*\)$");
		private readonly Type[] genericTypes;

		private readonly string symbolName;

		public TerminalAttribute(string symbolName): this(symbolName, null) {}

		public TerminalAttribute(string symbolName, params Type[] genericTypes) {
			if (string.IsNullOrEmpty(symbolName)) {
				throw new ArgumentNullException("symbolName");
			}
			this.symbolName = rxSpecialToken.IsMatch(symbolName) ? symbolName : Symbol.FormatTerminalSymbol(symbolName);
			this.genericTypes = genericTypes ?? Type.EmptyTypes;
		}

		public Type[] GenericTypes {
			get {
				return genericTypes;
			}
		}

		public bool IsGeneric {
			get {
				return genericTypes.Length > 0;
			}
		}

		public string SymbolName {
			get {
				return symbolName;
			}
		}

		public Symbol Bind(CompiledGrammar grammar) {
			if (grammar == null) {
				throw new ArgumentNullException("grammar");
			}
			Symbol result;
			grammar.TryGetSymbol(symbolName, out result);
			return result;
		}
	}
}