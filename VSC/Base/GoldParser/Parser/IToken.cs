using System;

using VSC.Base.GoldParser.Grammar;

namespace VSC.Base.GoldParser.Parser {
	/// <summary>
	/// The common interface for all tokens, which carries the (grammar) symbol as well as their position in the input text.
	/// </summary>
	public interface IToken {
		/// <summary>
		/// Gets the line number where this token begins.
		/// </summary>
		/// <value>The line number and position.</value>
		LineInfo Position {
			get;
		}

		/// <summary>
		/// Gets the symbol associated with this token.
		/// </summary>
		/// <value>The parent symbol.</value>
		Symbol Symbol {
			get;
		}

		/// <summary>
		/// Checks if the symbol name is the given name.
		/// </summary>
		/// <param name="name">The name to check.</param>
		/// <returns><c>true</c> if the name match, false otherwise</returns>
		bool NameIs(string name);
	}
}