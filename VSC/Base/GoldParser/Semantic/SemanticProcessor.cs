using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using VSC.Base.GoldParser.Grammar;
using VSC.Base.GoldParser.Parser;

namespace VSC.Base.GoldParser.Semantic {
	public class SemanticProcessor<T>: LalrProcessor<T> where T: SemanticToken {
		private readonly SemanticActions<T> actions;

        public SemanticProcessor(ParserReader reader, SemanticActions<T> actions) : this(actions.CreateTokenizer(reader), actions) { }

		public SemanticProcessor(ITokenizer<T> tokenizer, SemanticActions<T> actions): base(tokenizer) {
			
            if (actions == null) {
				throw new ArgumentNullException("actions");
			}
			if (tokenizer.Grammar != actions.Grammar) {
				throw new ArgumentException("Mismatch of tokenizer and action grammars");
			}
			this.actions = actions;
		}

		protected override bool CanTrim(Rule rule) {
			return false;
		}

		protected override T CreateReduction(Rule rule, IList<T> children) {
			SemanticNonterminalFactory<T> factory;
			if (actions.TryGetNonterminalFactory(rule, out factory)) {
				Debug.Assert(factory != null);
				return factory.CreateAndInitialize(rule, children);
			}
			throw new InvalidOperationException(string.Format("Missing a token type for the rule {0}", rule.Definition));
		}
	}
}