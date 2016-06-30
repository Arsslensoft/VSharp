using System;

namespace VSC.Base.GoldParser.Grammar {
	/// <summary>
	/// Type of symbol.
	/// </summary>
	public enum SymbolKind {
		/// <summary>
		/// Normal nonterminal
		/// </summary>
		Nonterminal = 0,

		/// <summary>
		/// Normal terminal
		/// </summary>
		Terminal = 1,

		/// <summary>
		/// This Whitespace symbols is a special terminal
		/// that is automatically ignored the the parsing engine.
		/// Any text accepted as whitespace is considered
		/// to be inconsequential and "meaningless".
		/// </summary>
		/// <remarks>This is now called "<see cref="Noise"/>".</remarks>
		[Obsolete("Starting with GOLD Builder V5, those are called 'Noise' terminals", false)]
		WhiteSpace = 2,

		/// <summary>
		/// This Whitespace symbols is a special terminal
		/// that is automatically ignored the the parsing engine.
		/// Any text accepted as noise is considered
		/// to be inconsequential and "meaningless".
		/// </summary>
		Noise = 2,

		/// <summary>
		/// The End symbol is generated when the tokenizer
		/// reaches the end of the source text.
		/// </summary>
		End = 3,

		/// <summary>
		/// This type of symbol designates the start of a block quote.
		/// </summary>
		/// <remarks>This is now called a "<see cref="BlockStart"/>".</remarks>
		[Obsolete("Starting with GOLD Builder V5, those are called 'BlockStart' terminals", false)]
		CommentStart = 4,

		/// <summary>
		/// This type of symbol designates the start of a block quote.
		/// </summary>
		BlockStart = 4,

		/// <summary>
		/// This type of symbol designates the end of a block quote.
		/// </summary>
		/// <remarks>This is now called a "<see cref="BlockEnd"/>".</remarks>
		[Obsolete("Starting with GOLD Builder V5, those are called 'BlockEnd' terminals", false)]
		CommentEnd = 5,

		/// <summary>
		/// This type of symbol designates the end of a block quote.
		/// </summary>
		BlockEnd = 5,

		/// <summary>
		/// When the engine reads a token that is recognized as
		/// a line comment, the remaining characters on the line
		/// are automatically ignored by the parser.
		/// </summary>
		/// <remarks>This is only there for legacy (CGT) grammar files.</remarks>
		[Obsolete("Starting with GOLD Builder V5, those are obsolete but remain valid for legacy (CGT) grammars.", false)]
		CommentLine = 6,

		/// <summary>
		/// The Error symbol is a general-purpose means
		/// of representing characters that were not recognized
		/// by the tokenizer. In other words, when the tokenizer
		/// reads a series of characters that is not accepted
		/// by the DFA engine, a token of this type is created.
		/// </summary>
		Error = 7
	}
}