using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using VSC.Base.GoldParser.Grammar;

namespace VSC.Base.GoldParser.Semantic {
	public sealed class SemanticTrimFactory<TBase>: SemanticNonterminalFactory<TBase> where TBase: SemanticToken {
		private readonly int handleIndex;
		private readonly SemanticActions<TBase> owner;
		private readonly Rule rule;

		internal SemanticTrimFactory(SemanticActions<TBase> owner, Rule rule, int handleIndex) {
			if (owner == null) {
				throw new ArgumentNullException("owner");
			}
			if (rule == null) {
				throw new ArgumentNullException("rule");
			}
			if ((handleIndex < 0) || (handleIndex >= rule.SymbolCount)) {
				throw new ArgumentOutOfRangeException("handleIndex");
			}
			this.owner = owner;
			this.handleIndex = handleIndex;
			this.rule = rule;
		}

		public override ReadOnlyCollection<Type> InputTypes {
			get {
				return Array.AsReadOnly(new[] {GetRuleType()});
			}
		}

		public override Type OutputType {
			get {
				return GetRuleType();
			}
		}

		protected internal override Symbol RedirectForOutputType {
			get {
				return GetTrimSymbol();
			}
		}

		public override TBase CreateAndInitialize(Rule rule, IList<TBase> tokens) {
			Debug.Assert(this.rule == rule);
			TBase result = tokens[handleIndex];
			Debug.Assert(OutputType.IsAssignableFrom(result.GetType()));
			return result;
		}

		protected internal override IEnumerable<Symbol> GetInputSymbols(Rule rule) {
			yield return GetTrimSymbol();
		}

		private Type GetRuleType() {
			if (owner.Initialized) {
				return owner.GetSymbolOutputType(GetTrimSymbol());
			}
			return typeof(TBase);
		}

		private Symbol GetTrimSymbol() {
			return rule[handleIndex];
		}
	}
}