using VSC.Base.GoldParser.Grammar;

namespace VSC.Base.GoldParser.Parser {
	internal class DummyGroup: IGroup {
		public static readonly DummyGroup Default = new DummyGroup();

		public GroupAdvanceMode AdvanceMode {
			get {
				return GroupAdvanceMode.Token;
			}
		}

		public Symbol ContainerSymbol {
			get {
				return null;
			}
		}

		public Symbol EndSymbol {
			get {
				return null;
			}
		}

		public GroupEndingMode EndingMode {
			get {
				return GroupEndingMode.Open;
			}
		}

		public bool IsAllowedDfaState(DfaState state) {
			return true;
		}

		public bool IsNestingAllowed(Group group) {
			return true;
		}
	}
}
