using System;
using System.Diagnostics;

using VSC.Base.GoldParser.Grammar;
using VSC.Base.GoldParser.Parser;

namespace VSC.Base.GoldParser.Semantic {
	/// <summary>
	/// The abstract nongeneric case class for semantic terminal tokens. This class is for internal use only.
	/// </summary>
	/// <typeparam name="TBase">The base type of the semantic token.</typeparam>
	public abstract class SemanticTerminalFactory<TBase>: SemanticTokenFactory<TBase> where TBase: SemanticToken {
		internal SemanticTerminalFactory() {}

		public abstract TBase CreateAndInitialize(Symbol symbol, LineInfo position, string text);
	}

	/// <summary>
	/// The abstract generic case class for semantic terminal tokens. This class is usually not directly inherited.
	/// </summary>
	/// <typeparam name="TBase">The base type of the semantic token.</typeparam>
	/// <typeparam name="TOutput">The type of the terminal token.</typeparam>
	public abstract class SemanticTerminalFactory<TBase, TOutput>: SemanticTerminalFactory<TBase> where TBase: SemanticToken where TOutput: TBase {
		public override sealed Type OutputType {
			get {
				return typeof(TOutput);
			}
		}

		public override sealed TBase CreateAndInitialize(Symbol symbol, LineInfo position, string text) {
			Debug.Assert(symbol != null);
			TOutput result = Create(text);
			Debug.Assert(result != null);
			result.Initialize(symbol, position);
			return result;
		}

		protected abstract TOutput Create(string text);
	}
}