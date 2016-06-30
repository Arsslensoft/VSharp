using System;

using VSC.Base.GoldParser.Grammar;
using VSC.Base.GoldParser.Parser;

namespace VSC.Base.GoldParser.Semantic {
	public abstract class RuleAttributeBase: Attribute {
		private readonly Reduction parsedRule;

		// This gets rid of the CLS compliance warning without introducing a security problem by inherited attributes
		protected RuleAttributeBase() {
			throw new NotSupportedException("This class is not intended to be inherited");
		}

		internal RuleAttributeBase(string rule) {
			if (string.IsNullOrEmpty(rule)) {
				throw new ArgumentNullException("rule");
			}
			if (!RuleDeclarationParser.TryParse(rule, out parsedRule)) {
				throw new ArgumentException(string.Format("The rule {0} contains a syntax error", rule), "rule");
			}
		}

		public string Rule {
			get {
				return parsedRule.ToString();
			}
		}

		internal Reduction ParsedRule {
			get {
				return parsedRule;
			}
		}

		public Rule Bind(CompiledGrammar grammar) {
			if (grammar == null) {
				throw new ArgumentNullException("grammar");
			}
			Rule rule;
			RuleDeclarationParser.TryBindGrammar(parsedRule, grammar, out rule);
			return rule;
		}
	}
}