using System;
using System.Diagnostics;

namespace VSC.Base.GoldParser.Grammar {
	internal abstract class LalrActionWithLalrState: LalrAction {
		private readonly LalrState state;

		protected LalrActionWithLalrState(int index, Symbol symbol, LalrState state): base(index, symbol) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			Debug.Assert(symbol.Owner == state.Owner);
			this.state = state;
		}

		public LalrState State {
			get {
				return state;
			}
		}

		public override object Target {
			get {
				return state;
			}
		}
	}
}
