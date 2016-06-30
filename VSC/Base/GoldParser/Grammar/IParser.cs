using System;

using VSC.Base.GoldParser.Parser;

namespace VSC.Base.GoldParser.Grammar {
	public interface IParser<T> where T: IToken {
		LalrState TopState {
			get;
		}

		bool CanTrim(Rule rule);
		T CreateReduction(Rule rule);
		T PopToken();
		void PushTokenAndState(T token, LalrState state);
	}
}