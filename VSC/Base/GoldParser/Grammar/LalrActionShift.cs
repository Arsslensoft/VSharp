using System;

using VSC.Base.GoldParser.Parser;

namespace VSC.Base.GoldParser.Grammar {
	internal sealed class LalrActionShift: LalrActionWithLalrState {
		public LalrActionShift(int index, Symbol symbol, LalrState state): base(index, symbol, state) {}

		public override LalrActionType ActionType {
			get {
				return LalrActionType.Shift;
			}
		}

		internal override TokenParseResult Execute<T>(IParser<T> parser, T token) {
			parser.PushTokenAndState(token, State);
			return TokenParseResult.Shift;
		}
	}
}