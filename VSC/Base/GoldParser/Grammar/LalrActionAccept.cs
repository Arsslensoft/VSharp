using System;

using VSC.Base.GoldParser.Parser;

namespace VSC.Base.GoldParser.Grammar {
	internal sealed class LalrActionAccept: LalrAction {
		public LalrActionAccept(int index, Symbol symbol): base(index, symbol) {}

		public override LalrActionType ActionType {
			get {
				return LalrActionType.Accept;
			}
		}

		internal override TokenParseResult Execute<T>(IParser<T> parser, T token) {
			return TokenParseResult.Accept;
		}
	}
}