using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using VSC.Base.GoldParser.Grammar;
using VSC.Base.GoldParser.Parser;

namespace VSC.Base.GoldParser.Semantic {
	/// <summary>
	/// The abstract nongeneric case class for semantic nonterminal tokens. This class is for internal use only.
	/// </summary>
	/// <typeparam name="TBase">The base type of the semantic token.</typeparam>
	public abstract class SemanticNonterminalFactory<TBase>: SemanticTokenFactory<TBase> where TBase: SemanticToken {
		public abstract ReadOnlyCollection<Type> InputTypes {
			get;
		}

		public abstract TBase CreateAndInitialize(Rule rule, IList<TBase> tokens);
		protected internal abstract IEnumerable<Symbol> GetInputSymbols(Rule rule);
	}

	/// <summary>
	/// The abstract generic case class for semantic nonterminal tokens. This class is usually not directly inherited.
	/// </summary>
	/// <typeparam name="TBase">The base type of the semantic token.</typeparam>
	/// <typeparam name="TOutput">The type of the nonterminal token.</typeparam>
	public abstract class SemanticNonterminalFactory<TBase, TOutput>: SemanticNonterminalFactory<TBase> where TBase: SemanticToken where TOutput: TBase {
		public override sealed Type OutputType {
			get {
				return typeof(TOutput);
			}
		}

		public abstract TOutput Create(Rule rule, IList<TBase> tokens);

		public override sealed TBase CreateAndInitialize(Rule rule, IList<TBase> tokens) {
			Debug.Assert(rule != null);
			TOutput result = Create(rule, tokens);
			Debug.Assert(result != null);
			LineInfo position = default(LineInfo);
			for (int i = 0; i < tokens.Count; i++) {
				IToken token = tokens[i];
				if (token.Position.Index > 0) {
					position = token.Position;
					break;
				}
			}
			result.Initialize(rule.RuleSymbol, position);
			return result;
		}

		protected internal override IEnumerable<Symbol> GetInputSymbols(Rule rule) {
			if (rule == null) {
				throw new ArgumentNullException("rule");
			}
			return rule;
		}
	}
}